using Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Server.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Post> Post { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<UpVote> UpVote { get; set; }
        public DbSet<DownVote> DownVote { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UpVote>(entity =>
            {
                entity.HasKey(e => new { e.PostId, e.UserName });
            });

            modelBuilder.Entity<DownVote>(entity =>
            {
                entity.HasKey(e => new { e.PostId, e.UserName });
            });
        }
    }
}