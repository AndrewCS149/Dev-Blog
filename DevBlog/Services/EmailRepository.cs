using DevBlog.Data;
using DevBlog.Interfaces;
using DevBlog.Models;
using Microsoft.AspNetCore.Identity;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace DevBlog.Services
{
    public class EmailRepository : IEmailRepository
    {
        private readonly IConfiguration _config;
        private readonly UserManager<UserModel> userMgr;
        private readonly UserDbContext _userDb;
        private readonly EmailAddress _email = new EmailAddress("devmaster@thedevblog.net");

        public EmailRepository(UserDbContext userdb, IConfiguration config, UserManager<UserModel> um)
        {
            _userDb = userdb;
            userMgr = um;
            _config = config;
        }

        /// <summary>
        /// Emails a welcome message to a newly registered user
        /// </summary>
        /// <param name="email">User's email</param>
        /// <returns>Successful completion of task</returns>
        public async Task Welcome(string email)
        {
            var apiKey = _config["SendgridApiKey"];
            var client = new SendGridClient(apiKey);

            var msg = new SendGridMessage()
            {
                TemplateId = _config["SendgridWelcomeTemplate"],
                From = _email,
            };

            msg.AddTo(email);

            await client.SendEmailAsync(msg);
        }

        /// <summary>
        /// Sends an email to all subscribed users when a new post is made
        /// </summary>
        /// <param name="img">The image to attach</param>"
        /// <returns>Successful completion of task</returns>
        public async Task NewPost(string img = null)
        {
            var apiKey = _config["SendgridApiKey"];
            var client = new SendGridClient(apiKey);

            var msg = new SendGridMessage()
            {
                TemplateId = _config["SendgridNewPostTemplate"],
                From = _email
            };

            // get all users who are subscribed
            string adminUser = _config["AdminUsername"];
            List<UserModel> users = userMgr.Users.Where(x => x.UserName != adminUser && x.Subscribed).ToList();

            if (users.Count > 0)
            {
                List<EmailAddress> emails = new List<EmailAddress>();
                users.ForEach(x => emails.Add(new EmailAddress(x.Email)));
                msg.AddTos(emails);
                await client.SendEmailAsync(msg);
            }
        }

        /// <summary>
        /// Checks whether an email already exists
        /// </summary>
        /// <param name="email"></param>
        /// <returns>bool</returns>
        public bool CheckEmail(string email)
        {
            var emails = _userDb.Users.Select(x => x.NormalizedEmail).ToList();
            return emails.Contains(email.ToUpper()) ? true : false;
        }

        /// <summary>
        /// Checks whether a username already exists
        /// </summary>
        /// <param name="username"></param>
        /// <returns>bool</returns>
        public bool CheckUsername(string username)
        {
            var users = _userDb.Users.Select(x => x.NormalizedUserName).ToList();
            return users.Contains(username.ToUpper()) ? true : false;
        }
    }
}