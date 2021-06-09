namespace SocialCode.API.Requests.Comments
{
    public class CommentRequest
    {
        public string Username { get; set; }
        public string PostId { get; set; }
        public string Content{ get; set; }
    }
}