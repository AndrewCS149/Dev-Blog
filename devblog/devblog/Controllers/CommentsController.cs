using devblog.Interfaces;
using devblog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace devblog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _comments;
        private readonly IPostService _posts;

        public CommentsController(ICommentService comments, IPostService posts)
        {
            _comments = comments;
            _posts = posts;
        }

        /// <summary>
        /// Adds a comment
        /// </summary>
        /// <param name="comment">The comment to add</param>
        /// <returns>Comment</returns>
        [Authorize]
        [HttpPost]
        public async Task<Comment> Create(Comment comment)
        {
            var newComment = await _comments.Create(comment.Content, comment.UserName, comment.PostId);
            return newComment;
        }

        /// <summary>
        /// Update a comment
        /// </summary>
        /// <param name="id">id of comment to update</param>
        /// <param name="content">new content of comment</param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("{id}")]
        public async Task<Comment> Update(int id, [FromBody] string content)
        {
            var newComment = await _comments.Update(id, content);
            return newComment;
        }

        /// <summary>
        /// Gets all comment from a specified post
        /// </summary>
        /// <param name="postId">Post id to retrieve comments for</param>
        /// <returns>List<Comment></returns>
        //[Authorize]
        [HttpGet("posts/{postId}")]
        public async Task<List<Comment>> Get(int postId)
        {
            var post = await _posts.Get(postId);
            var comments = post.Comments.OrderByDescending(c => c.Id).ToList();
            return comments != null ? comments : new List<Comment>();
        }

        /// <summary>
        /// Removes a specified comment
        /// </summary>
        /// <param name="id">comment Id</param>
        /// <returns>Successful completion of task</returns>
        [Authorize]
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await _comments.Delete(id);
        }
    }
}

