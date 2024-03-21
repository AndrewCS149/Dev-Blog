using devblog.Data;
using devblog.Interfaces;
using devblog.Models;
using Microsoft.EntityFrameworkCore;

namespace devblog.Services
{
    public class CommentService : ICommentService
    {
        private readonly AppDbContext _db;
        private readonly INotificationService _notifications;

        public CommentService(AppDbContext db, INotificationService notifications)
        {
            _notifications = notifications;
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
            await _notifications.CreateNewCommentNotification(postId, userName);

            return newComment;
        }

        /// <summary>
        /// Update a comment
        /// </summary>
        /// <param name="id">id of comment to update</param>
        /// <param name="content">new content of comment</param>
        /// <returns></returns>
        public async Task<Comment> Update(int id, string content)
        {
            var comment = await Get(id);
            comment.Content = content;
            _db.Comment.Update(comment);
            await _db.SaveChangesAsync();

            return comment;
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