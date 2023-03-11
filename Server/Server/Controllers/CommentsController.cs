using Server.Interfaces;
using Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _comments;

        public CommentsController(ICommentService comments)
        {
            _comments = comments;
        }

        /// <summary>
        /// Retrieves all comments  for a specified posts
        /// </summary>
        /// <param name="postId">Post Id</param>
        /// <returns>List<Comment></returns>
        [HttpGet("post/{postId}")]
        public async Task<List<Comment>> GetAllForPost(int postId)
        {
            var comments = await _comments.GetAllForPost(postId);
            return comments;
        }

        /// <summary>
        /// Adds a comment
        /// </summary>
        /// <param name="comment">The comment to add</param>
        /// <returns>Comment</returns>
        //[Authorize]
        [HttpPost]
        public async Task<Comment> Create(Comment comment)
        {
            var newComment = await _comments.Create(comment.Content, comment.UserName, comment.PostId);
            return newComment;
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

