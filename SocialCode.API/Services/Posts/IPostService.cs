using System.Collections.Generic;
using System.Threading.Tasks;
using SocialCode.API.Requests;
using SocialCode.API.Requests.Posts;

namespace SocialCode.API.Services.Posts
{
    public interface IPostService
    {
        Task<SocialCodeResult<PostResponse>> Insert(PostRequest insertRequest);
        Task<SocialCodeResult<PostResponse>> GetPostById(string id);
        Task<SocialCodeResult<PostResponse>> DeletePost(string id);
        Task<SocialCodeResult<PostResponse>> ModifyPost(string id, PostRequest updatedPost);
        Task<SocialCodeResult<IEnumerable<PostResponse>>> GetAllUserPosts(string id);
    }
}