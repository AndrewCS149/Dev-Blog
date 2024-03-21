using devblog.Interfaces;
using Mastonet.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace devblog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notifications;
        private readonly IImgService _imgs;

        public NotificationsController(INotificationService notifications, IImgService imgs)
        {
            _notifications = notifications;
            _imgs = imgs;
        }

        /// <summary>
        /// Gets all notifications for a specific user
        /// </summary>
        /// <param name="userName">Username to retrieve unseen notifications for</param>
        /// <returns>List<Notification></returns>
        [Authorize]
        [HttpGet("{userName}")]
        public async Task<List<Models.Notification>> Get(string userName)
        {
            var notifications = await _notifications.Get(userName);
            return notifications.OrderByDescending(n => n.PostId).ToList();
        }

        /// <summary>
        /// Delete a specified notification
        /// </summary>
        [Authorize]
        [HttpDelete("{postId}/{userName}")]
        public async Task Delete(int postId, string userName)
        {
            await _notifications.Delete(postId, userName);
        }
    }
}
