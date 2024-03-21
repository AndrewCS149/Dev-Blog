using devblog.Data;
using devblog.Interfaces;
using devblog.Models;
using Microsoft.EntityFrameworkCore;
using Discord;
using Discord.WebSocket;
using Mastonet;
using System.Net;
using devblog.Controllers;

namespace devblog.Services
{
    public class PostService : IPostService
    {
        private readonly AppDbContext _db;
        private readonly IImgService _imgs;
        private readonly IConfiguration _config;
        private readonly DiscordSocketClient _discordClient;
        private readonly INotificationService _notifications;
        private readonly IEmailService _email;

        public PostService(AppDbContext context, IImgService imgService, DiscordSocketClient discordClient, IConfiguration config, INotificationService notificationService, IEmailService email)
        {
            _db = context;
            _imgs = imgService;
            _notifications = notificationService;
            _config = config;
            _email = email;

            // set up discord client
            _discordClient = discordClient;
            _discordClient.LoginAsync(TokenType.Bot, _config.GetValue<string>("DiscordToken"));
        }

        /// <summary>
        /// Creates a new post
        /// </summary>
        /// <param name="post">Data for new post</param>
        /// <returns>UploadStatus</returns>
        public async Task<UploadStatus> Create(PostUpload post)
        {
            var newPost = new Post()
            {
                Date = DateTime.Now,
                Description = post.description,
            };

            var uploadStatus = new UploadStatus();

            if (post.postToDiscord)
                uploadStatus.DiscordStatus = await PostToDiscord(post.description, post.files);

            if (post.postToMastodon)
                uploadStatus.MastodonStatus = await PostToMastodon(post.description, post.files);

            if (post.postToDevBlog)
            {
                var res = _db.Post.Add(newPost).Entity;
                await _db.SaveChangesAsync();
                uploadStatus.DevBlogStatus = await _imgs.Create(post.files, res.Id);

                // create notifications and send emails for new post
                await _notifications.CreatePostNotification(res.Id, res.Imgs[0].Url);
                await _email.NewPost();
            }

            return uploadStatus;
        }

        /// <summary>
        /// Gets the total post count
        /// </summary>
        /// <returns>int</returns>
        public async Task<int> GetPostCount()
        {
            int postCount = await _db.Post.CountAsync();
            return postCount;
        }

        /// <summary>
        /// Gets the total page count
        /// </summary>
        /// <returns>int</returns>
        public async Task<int> GetPageCount()
        {
            int postCount = await _db.Post.CountAsync();
            int pageCount = (int)Math.Ceiling(postCount / 5.0);
            return pageCount;
        }

        /// <summary>
        /// Retrieves all posts (5 max) for specified page
        /// </summary>
        /// <param name="pageNum">The page number to get posts from</param>
        /// <returns>List<Post></returns>
        public async Task<List<Post>> GetPage(int pageNum)
        {
            var posts = await _db.Post.OrderByDescending(x => x.Date)
                                      .Include(x => x.Comments.OrderByDescending(c => c.Id))
                                      .Include(x => x.Imgs)
                                      .Include(x => x.UpVotes)
                                      .Include(x => x.DownVotes)
                                      .Skip((pageNum - 1) * 5)
                                      .Take(5)
                                      .ToListAsync();

            return posts;
        }

        /// <summary>
        /// Returns the page number of a given post
        /// </summary>
        public async Task<int> GetPageNumber(int postId)
        {
            int pageSize = 5;

            var posts = await _db.Post.OrderByDescending(x => x.Date).ToListAsync();
            var postIdx = posts.FindIndex(x => x.Id == postId);

            if (postIdx >= 0)
            {
                int pageNum = (postIdx / pageSize) + 1;
                return pageNum;
            }

            // If post not found or encountered an error, return 0 or handle accordingly
            return 0;
        }

        /// <summary>
        /// Retrieves a specified post
        /// </summary>
        /// <param name="postId">Post Id</param>
        /// <returns>Post</returns>
        public async Task<Post> Get(int postId)
        {
            Post post;

            // if id is -1, get latest post, else get specified post
            if (postId == -1)
            {
                post = await _db.Post.OrderByDescending(x => x.Date)
                                         .Include(x => x.Comments.OrderByDescending(c => c.Id))
                                         .Include(x => x.Imgs)
                                         .Include(x => x.UpVotes)
                                         .Include(x => x.DownVotes)
                                         .FirstOrDefaultAsync();
            }
            else
            {
                post = await _db.Post.Include(x => x.Comments.OrderByDescending(c => c.Id))
                                         .Include(x => x.Imgs)
                                         .Include(x => x.UpVotes)
                                         .Include(x => x.DownVotes)
                                         .Where(p => p.Id == postId)
                                         .FirstOrDefaultAsync();
            }

            return post;
        }

        /// <summary>
        /// Updates a post
        /// </summary>
        /// <param name="id">Id of post to be updated</param>
        /// <param name="description">New description of post</param>
        /// <returns>Successful completion of task</returns>
        public async Task<Post> Update(int id, string description)
        {
            var post = await _db.Post.Where(p => p.Id == id).FirstOrDefaultAsync();
            post.Description = description;
            _db.Post.Update(post);
            await _db.SaveChangesAsync();

            return post;
        }

        /// <summary>
        /// Removes a specified post
        /// </summary>
        /// <param name="postId">Post Id</param>
        /// <returns>Successful completion of task</returns>
        public async Task Delete(int postId)
        {
            var post = await Get(postId);
            await _notifications.DeleteAllForPost(postId);
            _db.Remove(post);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Sends new post to discord
        /// </summary>
        /// <param name="description">Description of new post</param>
        /// <param name="files">Images of new post</param>
        private async Task<HttpResponseMessage> PostToDiscord(string description, IFormFile[] files)
        {
            await _discordClient.StartAsync().WaitAsync(TimeSpan.FromSeconds(15));

            var channel = await _discordClient.GetChannelAsync(_config.GetValue<ulong>("DiscordChannelId")) as IMessageChannel;
            var res = new HttpResponseMessage();

            List<string> descriptions = new List<string>();
            int posts = (int)Math.Ceiling(description.Length / 1988.0);

            // add imgs to request
            List<FileAttachment> attachments = new List<FileAttachment>();
            foreach (var file in files)
                attachments.Add(new FileAttachment(file.OpenReadStream(), file.FileName));

            try
            {
                // because discords post chararacter limit is 2000, the description needs to broken into incriments 
                // of 2000 (including the 'part' string variable below)
                for (int i = 1; i < posts; i++)
                {
                    string postDescription = description.Substring(0, 1988);
                    description = description.Remove(0, 1988);
                    descriptions.Add(postDescription);
                }

                descriptions.Add(description);

                for (int i = 1; i <= descriptions.Count; i++)
                {
                    string part = $"(Part {i}/{descriptions.Count}) ";

                    if (i == 1)
                    {
                        await channel.SendFilesAsync(attachments, part + descriptions[i - 1]);
                    }
                    else
                    {
                        await channel.SendMessageAsync(part + descriptions[i - 1]);
                    }
                }

                await _discordClient.StopAsync();
            }
            catch (Exception e)
            {
                res.StatusCode = HttpStatusCode.BadRequest;
                res.ReasonPhrase = "Failed to post to  Discord: " + e.Message;
                return res;
            }

            res.StatusCode = HttpStatusCode.OK;
            return res;
        }

        /// <summary>
        /// Sends new post to Mastodon
        /// </summary>
        /// <param name="description">Description of new post</param>
        /// <param name="files">Images of new post</param>
        private async Task<HttpResponseMessage> PostToMastodon(string description, IFormFile[] files)
        {
            var client = new MastodonClient("mastodon.social", _config.GetValue<string>("MastodonToken"));
            var res = new HttpResponseMessage();

            // add imgs to request
            List<string> attachments = new List<string>();
            List<string> descriptions = new List<string>();
            int posts = (int)Math.Ceiling(description.Length / 488.0);

            try
            {
                // because mastodons post chararacter limit is 500, the description needs to broken into incriments 
                // of 500 (including the 'part' string variable below)
                for (int i = 1; i < posts; i++)
                {
                    string postDescription = description.Substring(0, 488);
                    description = description.Remove(0, 488);
                    descriptions.Add(postDescription);
                }

                descriptions.Add(description);

                for (int i = 1; i <= descriptions.Count; i++)
                {
                    string part = $"(Part {i}/{descriptions.Count}) ";

                    if (i == 1)
                    {
                        foreach (var file in files)
                        {
                            var media = new MediaDefinition(file.OpenReadStream(), file.FileName);
                            var mediaId = await client.UploadMedia(media);
                            attachments.Add(mediaId.Id);
                        }
                        await client.PublishStatus(part + descriptions[i - 1], mediaIds: attachments);
                    }
                    else
                    {
                        await client.PublishStatus(part + descriptions[i - 1]);
                    }
                }
            }
            catch (Exception e)
            {
                res.StatusCode = HttpStatusCode.BadRequest;
                res.ReasonPhrase = "Failed to post to  Mastodon: " + e.Message;
                return res;
            }

            res.StatusCode = HttpStatusCode.OK;
            return res;
        }
    }
}