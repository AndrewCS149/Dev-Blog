namespace DevBlog.Interfaces
{
    public interface IEmailRepository
    {
        /// <summary>
        /// Emails a welcome message to a newly registered user
        /// </summary>
        /// <param name="email">User's email</param>
        /// <returns>Successful completion of task</returns>
        Task Welcome(string email);

        /// <summary>
        /// Sends an email to all subscribed users when a new post is made
        /// </summary>
        /// <param name="img">The image to attach</param>"
        /// <returns>Successful completion of task</returns>
        Task NewPost(string img = null);

        /// <summary>
        /// Checks whether a username already exists
        /// </summary>
        /// <param name="username"></param>
        /// <returns>bool</returns>
        bool CheckUsername(string username);

        /// <summary>
        /// Checks whether an email already exists
        /// </summary>
        /// <param name="email"></param>
        /// <returns>bool</returns>
        bool CheckEmail(string email);
    }
}