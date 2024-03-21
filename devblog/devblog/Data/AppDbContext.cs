using devblog.Models;
using Microsoft.EntityFrameworkCore;

namespace devblog.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Post> Post { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<UpVote> UpVote { get; set; }
        public DbSet<DownVote> DownVote { get; set; }
        public DbSet<Img> Img { get; set; }
        public DbSet<YtVideo> YtVideo { get; set; }
        public DbSet<Notification> Notification { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UpVote>().HasKey(e => new { e.PostId, e.UserName });

            modelBuilder.Entity<DownVote>().HasKey(e => new { e.PostId, e.UserName });

            modelBuilder.Entity<YtVideo>(entity =>
            {
                entity.HasData(new YtVideo { Id = 1, Url = "https://www.youtube.com/embed/DtuqZ11RhIc" });
                entity.HasIndex(y => y.Url).IsUnique();
            });
        }
    }
}