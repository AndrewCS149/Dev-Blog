namespace devblog.Models
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
            DateTime dateTime = DateTime.UtcNow;
            string dateTimeStr = dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff");
            Date = DateTime.Parse(dateTimeStr);
        }
    }
}