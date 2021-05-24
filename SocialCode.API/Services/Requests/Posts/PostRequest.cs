using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;


namespace SocialCode.API.Services.Requests.Posts
{
    public class PostRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        
        public string Timestamp { get; set; }
        public string Code { get; set; }
        public bool IsFree { get; set; }
        public int Price { get; set; }
        public string Author_Id { get; set; }
        public IEnumerable<string> Comments_Id { get; set; }
        public List<string> Tags;
    }
}