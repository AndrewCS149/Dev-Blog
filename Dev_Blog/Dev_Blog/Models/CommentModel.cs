using System;

namespace Dev_Blog.Models
{
    public class CommentModel
    {
        public int Id { get; set; }
        public int PostModelId { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; private set; } = DateTime.UtcNow;
        public string UserName { get; set; }
    }
}