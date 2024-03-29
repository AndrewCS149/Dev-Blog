﻿using devblog.Models;
using SendGrid;

namespace devblog.Interfaces
{
    public interface IEmailService
    {
        /// <summary>
        /// Emails a welcome message to a newly registered user
        /// </summary>
        Task Welcome(string email);

        /// <summary>
        /// Sends an email whenever a new post is made
        /// </summary>
        Task NewPost();

        /// <summary>
        /// Subscribes a user to the email list
        /// </summary>
        Task<Response> EmailSubscribe(string email);

        /// <summary>
        /// Toggles a specific users email preference
        /// </summary>
        Task<bool> ToggleSubscribe(User user);
    }
}
