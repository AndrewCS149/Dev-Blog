using DevBlog.Models;

namespace DevBlog.Interfaces
{
    public interface ICommentRepository
    {
        /// <summary>
        /// Adds a comment
        /// </summary>
        /// <param name="comment">The comment to add</param>
        /// <returns>CommentModel</returns>
        //Task<CommentModel> Create(CommentVM comment);

        /// <summary>
        /// Updates a specified comment
        /// </summary>
        /// <param name="comment">Comment Model</param>
        /// <returns>Successful completion of task</returns>
        Task Update(CommentModel comment);

        /// <summary>
        /// Retrieves all comments
        /// </summary>
        /// <returns>List<CommentModel></returns>
        Task<List<CommentModel>> GetComments();

        /// <summary>
        /// Retrieves a specified comment
        /// </summary>
        /// <param name="commentId">Comment Id</param>
        /// <returns>CommentModel</returns>
        Task<CommentModel> GetComment(int id);

        /// <summary>
        /// Deletes a specified comment
        /// </summary>
        /// <param name="commentId">Comment Id</param>
        /// <returns>Successful completion of task</returns>
        Task Delete(int id);
    }
}