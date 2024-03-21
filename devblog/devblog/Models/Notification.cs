namespace devblog.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public required string UserName { get; set; }
        public required string Author { get; set; }
        public required string ImgUrl { get; set; }
        public required string NotificationType { get; set; }
    }
}
