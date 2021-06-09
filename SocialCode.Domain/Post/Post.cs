using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace SocialCode.Domain.Post
{
    public class Post
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Title { get; set; }
        
        public IEnumerable<string> Tags;
        public string Description { get; set; }
        public string Timestamp { get; set; }
        public string Code { get; set; }
        public bool IsFree { get; set; }
        public bool IsDeleted { get; set; }
        public int Price { get; set; }
        public string AuthorID { get; set; }
    }
}