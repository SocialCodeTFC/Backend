namespace SocialCode.API.Requests.Comments
{
    public class CommentResponse
    {
        public string Content{ get; set; }
        public string Timestamp{ get; set; }
        public string AuthorUsername { get; set; }
    }
}