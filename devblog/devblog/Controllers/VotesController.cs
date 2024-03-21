using devblog.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace devblog.Controllers
{
    [ApiController]
    [Route("api/posts")]
    public class VotesController : ControllerBase
    {
        private readonly IVoteService _votes;

        public VotesController(IVoteService votes)
        {
            _votes = votes;
        }

        /// <summary>
        /// Returns the number of upvotes for specific post
        /// </summary>
        /// <param name="id">id of post</param>
        /// <returns>number of upvotes</returns>
        [HttpGet("{id}/upvotes")]
        public async Task<int> GetUpVotesForPost(int id)
        {
            var votes = await _votes.GetUpVotesForPost(id);
            return votes;
        }

        /// <summary>
        /// Returns the number of downvotes for specific post
        /// </summary>
        /// <param name="id">id of post</param>
        /// <returns>number of downvotes</returns>
        [HttpGet("{id}/downvotes")]
        public async Task<int> GetDownVotesForPost(int id)
        {
            var votes = await _votes.GetDownVotesForPost(id);
            return votes;
        }

        /// <summary>
        /// Adds an up vote
        /// </summary>
        /// <param name="vote"></param>
        /// <returns>Number of votes for post</returns>
        [Authorize]
        [HttpPost("{id}/upvote")]
        public async Task<VoteCount> UpVote(int id)
        {
            var username = User.FindFirstValue("userName");
            var upVotes = await _votes.UpVote(id, username);
            var downVotes = await _votes.GetDownVotesForPost(id);
            VoteCount voteCount = new VoteCount(upVotes, downVotes);
            return voteCount;
        }

        /// <summary>
        /// Adds a down vote
        /// </summary>
        /// <param name="vote"></param>
        /// <returns>Number of votes for post</returns>
        [Authorize]
        [HttpPost("{id}/downvote")]
        public async Task<VoteCount> DownVote(int id)
        {
            var username = User.FindFirstValue("userName");
            var downVotes = await _votes.DownVote(id, username);
            var upVotes = await _votes.GetUpVotesForPost(id);
            VoteCount voteCount = new VoteCount(upVotes, downVotes);
            return voteCount;
        }

        public class VoteCount
        {
            public int Up { get; set; }
            public int Down { get; set; }
            public VoteCount(int up, int down)
            {
                Up = up; 
                Down = down;
            }
        }
    }
}

