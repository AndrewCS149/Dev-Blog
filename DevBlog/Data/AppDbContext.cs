using DevBlog.Models;
using Microsoft.EntityFrameworkCore;

namespace DevBlog.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<PostModel> Post { get; set; }
        public DbSet<CommentModel> Comment { get; set; }
        public DbSet<UpVoteModel> UpVote { get; set; }
        public DbSet<DownVoteModel> DownVote { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UpVoteModel>(entity =>
            {
                entity.HasKey(e => new { e.PostModelId, e.UserName });
            });

            modelBuilder.Entity<DownVoteModel>(entity =>
            {
                entity.HasKey(e => new { e.PostModelId, e.UserName });
            });
        }
    }
}