using devblog.Data;
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
        public async Task CreatePostNotification(int postId, string imgUrl)
        {
            var allUsers = await _userMgr.Users.ToListAsync();

            allUsers.ForEach(async user =>
            {
                var notification = new Notification
                {
                    UserName = user.UserName,
                    PostId = postId,
                    Author = "DevMaster",
                    ImgUrl = imgUrl,
                    NotificationType = "post",
                };

                await _db.Notification.AddAsync(notification);
            });

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Creates a notification for users who have commented on a post
        /// </summary>
        /// <param name="author">Original author of notification content</param>
        public async Task CreateNewCommentNotification(int postId, string author)
        {
            var imgs = _db.Img.Where(i => i.PostId == postId).ToList();

            // get all unique comments on post by username
            var comments = _db.Comment
                 .Where(c => c.PostId == postId && c.UserName != author)
                 .GroupBy(c => c.UserName)
                 .Select(group => group.First())
                 .ToList();

            // add a notification for devmaster if the author is not devmaster
            if(!comments.Any(c => c.UserName.ToLower() == "devmaster") && author.ToLower() != "devmaster")
            {
                comments.Add(new Comment() { UserName = "DevMaster" });
            }

            // add notification for each user
            foreach (var comment in comments)
            {
                var notification = new Notification
                {
                    UserName = comment.UserName,
                    PostId = postId,
                    Author = author,
                    ImgUrl = imgs[0].Url,
                    NotificationType = "comment"
                };

                await _db.Notification.AddAsync(notification);
                await _db.SaveChangesAsync();
            }

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
