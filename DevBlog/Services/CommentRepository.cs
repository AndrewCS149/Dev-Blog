using DevBlog.Data;
using DevBlog.Interfaces;
using DevBlog.Models;
using Microsoft.EntityFrameworkCore;
//using static DevBlog.Pages.Posts;

namespace DevBlog.Services
{
    public class CommentRepository : ICommentRepository
    {
        private readonly AppDbContext _db;

        public CommentRepository(AppDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Adds a comment
        /// </summary>
        /// <param name="comment">The comment to add</param>
        /// <returns>CommentModel</returns>
        //public async Task<CommentModel> Create(CommentVM comment)
        //{
        //    CommentModel newComment = new CommentModel()
        //    {
        //        UserName = comment.UserName,
        //        PostModelId = comment.PostModelId,
        //        Content = comment.Content
        //    };

        //    _db.Add(newComment);
        //    await _db.SaveChangesAsync();

        //    return newComment;
        //}

        /// <summary>
        /// Updates a specified comment
        /// </summary>
        /// <param name="comment">Comment Model</param>
        /// <returns>Successful completion of task</returns>
        public async Task Update(CommentModel comment)
        {
            _db.Comment.Update(comment);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves all comments
        /// </summary>
        /// <returns>List<CommentModel></returns>
        public async Task<List<CommentModel>> GetComments()
        {
            var comments = await _db.Comment.OrderByDescending(x => x.Date)
                                            .ToListAsync();
            return comments;
        }

        /// <summary>
        /// Retrieves a specified comment
        /// </summary>
        /// <param name="commentId">Comment Id</param>
        /// <returns>CommentModel</returns>
        public async Task<CommentModel> GetComment(int commentId)
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
            var comment = await GetComment(commentId);
            _db.Remove(comment);
            await _db.SaveChangesAsync();
        }
    }
}