using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SocialCode.Domain.User
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        
        [BsonIgnore]
        public string Token { get; set; }
        public IEnumerable<Post.Post> LikedPosts { get; set; }
        public IEnumerable<Post.Post> PurchasedPosts { get; set; }
        public IEnumerable<Comment.Comment> Comments { get; set; }
    }
}