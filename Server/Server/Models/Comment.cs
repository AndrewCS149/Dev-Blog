namespace Server.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; private set; }
        public string UserName { get; set; }

        public Comment()
        {
            Date = DateTime.UtcNow;
        }
    }
}