using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SocialCode.Domain.Comment
{
    public class Comment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string AuthorId { get; set; }
        public string PostId { get; set; }
        public string Content{ get; set; }
        public string Timestamp{ get; set; }
    }
}