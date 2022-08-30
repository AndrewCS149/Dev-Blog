namespace DevBlog.Models
{
    public class PostModel
    {
        public int Id { get; set; }
        public string UpdateNum { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string ImgURL { get; set; }
        public List<UpVoteModel> UpVotes { get; set; }
        public List<DownVoteModel> DownVotes { get; set; }
        public List<CommentModel> Comments { get; set; }
    }
}