using System.Collections.Generic;
using SocialCode.API.Requests.Comments;

namespace SocialCode.API.Requests.Posts
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
        public string AuthorName{ get; set; }
        public string AuthorUsername { get; set; }
        public IEnumerable<string> Tags { get; set; }
        
    }
}