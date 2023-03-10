using Server.Data;
using Server.Interfaces;
using Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Server.Services
{
    public class CommentService : ICommentService
    {
        private readonly AppDbContext _db;

        public CommentService(AppDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Creates a new comment
        /// </summary>
        /// <param name="content">The content of the comment</param>
        /// <param name="userName">userName of comment</param>
        /// <param name="postId">postId of comment</param>
        /// <returns>Comment</returns>
        public async Task<Comment> Create(string content, string userName, int postId)
        {
            Comment newComment = new Comment()
            {
                UserName = userName,
                PostId = postId,
                Content = content
            };

            _db.Comment.Add(newComment);
            await _db.SaveChangesAsync();

            return newComment;
        }

        /// <summary>
        /// Updates a specified comment
        /// </summary>
        /// <param name="comment">Comment Model</param>
        /// <returns>Successful completion of task</returns>
        public async Task Update(Comment comment)
        {
            _db.Comment.Update(comment);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves a specified comment
        /// </summary>
        /// <param name="commentId">Comment Id</param>
        /// <returns>Comment</returns>
        public async Task<Comment> Get(int commentId)
        {
            var comment = await _db.Comment.Where(c => c.Id == commentId)
                                           .FirstOrDefaultAsync();
            return comment;
        }

        /// <summary>
        /// Deletes a specified comment
        /// </summary>
        /// <param name="commentId">Comment Id</param>
        /// <returns>Successful completion of task</returns>
        public async Task Delete(int commentId)
        {
            var comment = await Get(commentId);
            _db.Remove(comment);
            await _db.SaveChangesAsync();
        }
    }
}