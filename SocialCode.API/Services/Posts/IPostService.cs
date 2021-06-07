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
        Task<SocialCodeResult<PostResponse>> DeletePost(string id, string userId);
        Task<SocialCodeResult<PostResponse>> ModifyPost(string id, PostRequest updatedPost, string userId);
        Task<SocialCodeResult<IEnumerable<PostResponse>>> GetAllUserPosts(string id);
        Task<SocialCodeResult<PaginatedResult<PostResponse>>> GetRecentPosts(int limit, int offset);
        Task<SocialCodeResult<PaginatedResult<PostResponse>>> GetPostsByTags(TagFilters tags, int limit, int offset);
        Task<SocialCodeResult<IEnumerable<PostResponse>>> GetUserSavedPosts(string userId);
        Task<SocialCodeResult<PostResponse>> AddPostToUserSavedPosts(SavePostRequest savePostRequest);

    }
}