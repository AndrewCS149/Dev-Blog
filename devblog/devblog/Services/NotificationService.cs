﻿using devblog.Data;
using devblog.Interfaces;
using devblog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace devblog.Services
{
    public class NotificationService : INotificationService
    {
        public UserManager<User> _userMgr { get; }
        private readonly AppDbContext _db;

        public NotificationService(AppDbContext db, UserManager<User> usermgr)
        {
            _userMgr = usermgr;
            _db = db;
        }

        /// <summary>
        /// Creates a notification for a new post to every user
        /// </summary>
        public async Task Create(int PostId, string notificationType)
        {
            var allUsers = await _userMgr.Users.ToListAsync();

            allUsers.ForEach(async user =>
            {
                var notification = new Notification
                {
                    NotificationType = notificationType,
                    PostId = PostId,
                    Seen = false,
                    UserName = user.UserName,
                };

                await _db.Notification.AddAsync(notification);
            });

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Gets all notifications for a specific user
        /// </summary>
        /// <returns>List<Notification></returns>
        public async Task<List<Notification>> Get(string userName)
        {
            var notifications = await _db.Notification.Where(n => n.UserName.ToLower() == userName.ToLower()).ToListAsync();
            return notifications;
        }

        /// <summary>
        /// Delete a specified notification
        /// </summary>
        public async Task Delete(int postId, string userName)
        {
            var notification = await _db.Notification.Where(n => n.PostId == postId && n.UserName == userName).FirstOrDefaultAsync();
            _db.Remove(notification);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Delete all notifications for a specified post
        /// </summary>
        /// <param name="postId"></param>
        public async Task DeleteAllForPost(int postId)
        {
            var notifications = await _db.Notification.Where(n => n.PostId == postId).ToListAsync();
            notifications.ForEach(n => _db.Remove(n));
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Delete all notifications for a specified user
        /// </summary>
        /// <param name="username"></param>
        public async Task DeleteAllForUser(string username)
        {
            var notifications = await _db.Notification.Where(n => n.UserName.ToLower() == username.ToLower()).ToListAsync();
            notifications.ForEach(n => _db.Remove(n));
            await _db.SaveChangesAsync();
        }
    }
}
