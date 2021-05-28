using System.Collections.Generic;
using System.Linq;
using SocialCode.API.Requests.Posts;
using SocialCode.Domain.Post;

namespace SocialCode.API.Converters
{
    public static class PostConverter
    {
        public static Post PostRequest_ToModifiedPost(PostRequest postRequest, Post originalPost)
        {
            if (postRequest is null || originalPost is null) return null;
                    
            return new Post
            {
                Id = originalPost.Id,
                Title = postRequest.Title,
                Code = postRequest.Code,
                Description = postRequest.Description,
                Price = postRequest.Price,
                IsFree = postRequest.IsFree,
                Tags = postRequest.Tags.ToList(),
                AuthorID = originalPost.AuthorID,
                IsDeleted = originalPost.IsDeleted,
                Comments = originalPost.Comments,
                Timestamp = originalPost.Timestamp
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

        public static Post PostRequest_ToPost(PostRequest postRequest)
        {
            return new Post
            {
                Title = postRequest.Title,
                Code = postRequest.Code,
                Description = postRequest.Description,
                Price = postRequest.Price,
                IsFree = postRequest.IsFree,
                Tags = postRequest.Tags.ToList(),
                IsDeleted = false
            };
        }
    }
}