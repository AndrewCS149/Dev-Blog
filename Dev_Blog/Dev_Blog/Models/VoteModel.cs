namespace Dev_Blog.Models
{
    public class VoteModel
    {
        public int PostModelId { get; set; }
        public string UserName { get; set; }
        public bool UpVote { get; set; }
        public bool DownVote { get; set; }
    }
}
