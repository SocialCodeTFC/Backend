namespace SocialCode.API.Services.Requests.Comments
{
    public class CommentRequest
    {
        public string AuthorId { get; set; }
        public string PostId { get; set; }
        public string Content{ get; set; }
    }
}