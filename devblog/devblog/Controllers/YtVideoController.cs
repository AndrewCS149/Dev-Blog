using devblog.Interfaces;
using devblog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace devblog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class YtVideoController : ControllerBase
    {
        private readonly IYtVideoService _video;

        public YtVideoController(IYtVideoService video)
        {
            _video = video;
        }

        /// <summary>
        /// Gets the URL for the homepage YouTube video
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<YtVideo> Get()
        {
            var video = await _video.Get();
            return video;
        }

        /// <summary>
        /// Updates the url of the homepage YouTube video
        /// </summary>
        /// <param name="url">new url for video</param>
        /// <returns>Updated Video</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<YtVideo> Set([FromBody] string url)
        {
              var video = await _video.Update(url);
            return video;
        }
    }
}
