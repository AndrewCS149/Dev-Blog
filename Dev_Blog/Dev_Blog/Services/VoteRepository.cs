using Dev_Blog.Data;
using Dev_Blog.Interfaces;
using Dev_Blog.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Dev_Blog.Services
{
    public class VoteRepository : IVoteRepository
    {
        private AppDbContext _db;

        public VoteRepository(AppDbContext context)
        {
            _db = context;
        }

        /// <summary>
        /// Adds an up vote
        /// </summary>
        /// <param name="vote"></param>
        /// <returns>Successful completion of task</returns>
        public async Task<UpVoteModel> UpVote(int postId, string username)
        {
            // check if user has already upvoted, if so, remove upvote and return
            var upVote = await _db.UpVote.Where(x => x.UserName == username).FirstOrDefaultAsync();
            if (upVote != null)
            {
                _db.Remove(upVote);
                await _db.SaveChangesAsync();
                return null;
            }

            // check if user has already downvoted, if so, remove downvote
            var downVote = await _db.DownVote.Where(x => x.UserName == username).FirstOrDefaultAsync();
            if (downVote != null) _db.Remove(downVote);

            // create new vote and add to db
            var newVote = new UpVoteModel()
            {
                PostModelId = postId,
                UserName = username
            };

            _db.Add(newVote);
            await _db.SaveChangesAsync();
            return newVote;
        }

        /// <summary>
        /// Adds a down vote
        /// </summary>
        /// <param name="vote"></param>
        /// <returns>Successful completion of task</returns>
        public async Task<DownVoteModel> DownVote(int postId, string username)
        {
            // check if user has already downvoted, if so, remove downvote and return
            var downVote = await _db.DownVote.Where(x => x.UserName == username).FirstOrDefaultAsync();

            if (downVote != null)
            {
                _db.Remove(downVote);
                await _db.SaveChangesAsync();
                return null;
            }

            // check if user has already upvoted, if so, remove upvote
            var upVote = await _db.UpVote.Where(x => x.UserName == username).FirstOrDefaultAsync();
            if (upVote != null) _db.Remove(upVote);

            // create new vote and add to db
            var newVote = new DownVoteModel()
            {
                PostModelId = postId,
                UserName = username
            };

            _db.Add(newVote);
            await _db.SaveChangesAsync();
            return newVote;
        }
    }
}