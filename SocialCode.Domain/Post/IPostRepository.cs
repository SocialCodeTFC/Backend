using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialCode.Domain.Post
{
    public interface IPostRepository
    {
        Task<Post> GetPostById(string id);
        Task<Post> Insert(Post post);
        Task<Post> DeletePost(string id);
        Task<Post> ModifyPost(Post updatedPost, string id);
        Task<IEnumerable<Post>> GetAllUserPosts(string userId);
        
    }
}