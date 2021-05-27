using System.Collections.Generic;

namespace SocialCode.API.Services.Requests.Posts
{
    public class PostResponse
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Timestamp { get; set; }
        public string Code { get; set; }
        public bool IsFree { get; set; }
        public int Price { get; set; }
        public IEnumerable<string> Tags { get; set; }
        
        public IEnumerable<string> CommentsId { get; set; }
    }
}