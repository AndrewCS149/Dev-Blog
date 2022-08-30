using DevBlog.Data;
using DevBlog.Interfaces;
using DevBlog.Models;
using Microsoft.EntityFrameworkCore;

namespace DevBlog.Services
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
            var downVote = await _db.DownVote.Where(x => x.PostModelId == postId &&
                          x.UserName == username)
                           .FirstOrDefaultAsync();

            var upVote = await _db.UpVote.Where(v => v.PostModelId == postId &&
                          v.UserName == username)
                           .FirstOrDefaultAsync();

            if (downVote != null)
                _db.Remove(downVote);
            else if (upVote != null)
            {
                _db.Remove(upVote);
                await _db.SaveChangesAsync();
                return null;
            }

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
            var downVote = await _db.DownVote.Where(x => x.PostModelId == postId &&
                          x.UserName == username)
                           .FirstOrDefaultAsync();

            var upVote = await _db.UpVote.Where(v => v.PostModelId == postId &&
                          v.UserName == username)
                           .FirstOrDefaultAsync();

            if (upVote != null)
                _db.Remove(upVote);
            else if (downVote != null)
            {
                _db.Remove(downVote);
                await _db.SaveChangesAsync();
                return null;
            }
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