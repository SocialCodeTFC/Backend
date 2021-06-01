using System.Collections.Generic;

namespace SocialCode.API.Requests.Posts
{
    public class PostRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public bool IsFree { get; set; }
        public int Price { get; set; }
        public string Author_Id { get; set; }
        public IEnumerable<string> Tags { get; set; }
        
        }
}