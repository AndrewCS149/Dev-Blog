using Server.Models;

namespace Server.Interfaces
{
    public interface ICommentService
    {
        /// <summary>
        /// Creates a new comment
        /// </summary>
        /// <param name="content">The content of the comment</param>
        /// <param name="userName">userName of comment</param>
        /// <param name="postId">postId of comment</param>
        /// <returns>Comment</returns>
        Task<Comment> Create(string content, string userName, int postId);

        /// <summary>
        /// Updates a specified comment
        /// </summary>
        /// <param name="comment">Comment Model</param>
        /// <returns>Successful completion of task</returns>
        Task Update(Comment comment);

        /// <summary>
        /// Retrieves a specified comment
        /// </summary>
        /// <param name="commentId">Comment Id</param>
        /// <returns>Comment</returns>
        Task<Comment> Get(int id);

        /// <summary>
        /// Retrieves all comments for a specified post
        /// </summary>
        /// <param name="postId">Post Id</param>
        /// <returns>List<Comment></returns>
        Task<List<Comment>> GetAllForPost(int postId);

        /// <summary>
        /// Deletes a specified comment
        /// </summary>
        /// <param name="commentId">Comment Id</param>
        /// <returns>Successful completion of task</returns>
        Task Delete(int id);
    }
}
