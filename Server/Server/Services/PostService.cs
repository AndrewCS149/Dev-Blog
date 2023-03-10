using Server.Data;
using Server.Interfaces;
using Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Server.Services
{
    public class PostService : IPostService
    {
        private AppDbContext _db;

        public PostService(AppDbContext context)
        {
            _db = context;
        }

        /// <summary>
        /// Creates a new post
        /// </summary>
        /// <param name="description">Description of post</param>
        /// <param name="imgURL">Img URL of post</param>
        /// <param name="updateNum">Update number of post</param>
        /// <returns>Post</returns>
        public async Task<Post> Create(string description, string imgURL, string updateNum)
        {
            //TODO: change to datetimeoffset.utcnow
            //post.Date = DateTimeOffset.UtcNow;

            var newPost = new Post()
            {
                Date = DateTime.Now,
                Description = description,
                ImgURL = imgURL,
                UpdateNum = updateNum
            };

            var res = _db.Post.Add(newPost).Entity;
            await _db.SaveChangesAsync();

            return res;
        }

        /// <summary>
        /// Retrieves all posts
        /// </summary>
        /// <returns>List<Post></returns>
        public async Task<List<Post>> Get()
        {
            var posts = await _db.Post.OrderByDescending(x => x.Date)
                                      .Include(x => x.Comments)
                                      //.Include(x => x.UpVotes)
                                      //.Include(x => x.DownVotes)
                                      .ToListAsync();

            return posts;
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
                                         .Include(x => x.Comments)
                                         //.Include(x => x.UpVotes)
                                         //.Include(x => x.DownVotes)
                                         .FirstOrDefaultAsync();
            }
            else
            {
                post = await _db.Post.Include(x => x.Comments)
                                         //.Include(x => x.UpVotes)
                                         //.Include(x => x.DownVotes)
                                         .Where(p => p.Id == postId)
                                         .FirstOrDefaultAsync();
            }

            return post;
        }

        /// <summary>
        /// Updates a post
        /// </summary>
        /// <param name="post"></param>
        /// <returns>Successful completion of task</returns>
        public async Task Update(Post post)
        {
            _db.Post.Update(post);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Removes a specified post
        /// </summary>
        /// <param name="postId">Post Id</param>
        /// <returns>Successful completion of task</returns>
        public async Task Delete(int postId)
        {
            var post = await Get(postId);
            _db.Remove(post);
            await _db.SaveChangesAsync();
        }
    }
}