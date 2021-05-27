using System.Collections.Generic;
using System.Linq;
using SocialCode.API.Services.Requests.Posts;
using SocialCode.Domain.Post;


namespace SocialCode.API.Services.Converters
{
    public static class PostConverter
    {
        public static Post PostRequest_ToPost(PostRequest postRequest)
        {
            if (postRequest is null) return null;
                    
            return new Post
            {
                Title = postRequest.Title,
                Code = postRequest.Code,
                Description = postRequest.Description,
                Price = postRequest.Price,
                IsFree = postRequest.IsFree,
                Tags = postRequest.Tags.ToList()
            };
        }
        public static PostResponse Post_ToPostResponse(Post post)
        {
            return new PostResponse
            {
                Id = post.Id,
                Code = post.Code,
                Description = post.Description,
                Price = post.Price,
                Title = post.Title,
                Timestamp = post.Timestamp,
                IsFree = post.IsFree,
                Tags = post.Tags,
                
            };
        }
        public static IEnumerable<PostResponse> PostList_ToPostResponseList(IEnumerable<Post> postsList)
        {
            var postResponseList = postsList.Select(Post_ToPostResponse).ToList();
            return postResponseList;
        }
    }
}