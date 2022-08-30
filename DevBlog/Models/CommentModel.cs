namespace DevBlog.Models
{
    public class CommentModel
    {
        public int Id { get; set; }
        public int PostModelId { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; private set; }
        public string UserName { get; set; }

        public CommentModel()
        {
            Date = DateTime.UtcNow;
        }
    }
}