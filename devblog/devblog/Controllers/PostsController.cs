using devblog.Interfaces;
using devblog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace devblog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _posts;
        private readonly IImgService _imgs;
        public PostsController(IPostService posts, IImgService imgs)
        {
            _posts = posts;
            _imgs = imgs;
        }

        [HttpGet("getPageNum/{postId}")]
        public async Task<int> GetPageNumber(int postId)
        {
            var pageNum = await _posts.GetPageNumber(postId);
            return pageNum;
        }

        [HttpGet]
        public async Task<List<Post>> GetPage([FromQuery(Name = "page")] int pageNum)
        {
            var posts = await _posts.GetPage(pageNum);
            return posts;
        }

        /// <summary>
        /// Gets the total page count
        /// </summary>
        /// <returns>int</returns>
        [HttpGet("countPages")]
        public async Task<int> GetPageCount()
        {
            var count = await _posts.GetPageCount();
            return count;
        }

        /// <summary>
        /// Gets the total post count
        /// </summary>
        /// <returns>int</returns>
        [HttpGet("countPosts")]
        public async Task<int> GetPostCount()
        {
            int count = await _posts.GetPostCount();
            return count;
        }

        /// <summary>
        /// Retrieves Specified Post
        /// </summary>
        /// <param name="id">Post Id</param>
        /// <returns>List<Post></returns>
        [HttpGet("{id}")]
        public async Task<Post> Get(int id)
        {
            var posts = await _posts.Get(id);
            return posts;
        }

        /// <summary>
        /// Adds a new post
        /// </summary>
        /// <param name="post">Data for new post</param>
        /// <returns>UploadStatus</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<UploadStatus> Create([FromForm] PostUpload post)
        {
            var newPost = await _posts.Create(post);
            return newPost;
        }

        /// <summary>
        /// Updates a post
        /// </summary>
        /// <param name="id">Id of post to be updated</param>
        /// <param name="description">New description of post</param>
        /// <returns>Updated post</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<Post> Update(int id, [FromBody] string description)
        {
            var post = await _posts.Update(id, description);
            return post;
        }

        /// <summary>
        /// Removes a specified post
        /// </summary>
        /// <param name="postId">Post Id</param>
        /// <returns>Successful completion of task</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            var imgs = _posts.Get(id).Result.Imgs;
            await _imgs.DeleteImgFromDropBox(imgs);
            await _posts.Delete(id);
        }
    }

    public class PostUpload
    {
        public bool postToDiscord { get; set; }
        public bool postToMastodon { get; set; }
        public bool postToDevBlog { get; set; }
        public string? description { get; set; }
        public required IFormFile[] files { get; set; }
    }
}

