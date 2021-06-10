using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialCode.Domain.Post
{
    public interface IPostRepository
    {
        Task<Post> GetPostById(string id);
        Task<IEnumerable<Post>> GetPostsByIds(IEnumerable<string> postIds);
        Task<Post> Insert(Post post);
        Task<Post> DeletePost(string id);
        Task<Post> ModifyPost(Post updatedPost, string id);
        Task<IEnumerable<Post>> GetAllUserPosts(string userId);
        Task<IEnumerable<Post>> GetRecentPosts(int limit, int offset);
        Task<IEnumerable<Post>> GetPostByTagFilter(List<string> tags, int limit, int offset);
        

    }
}