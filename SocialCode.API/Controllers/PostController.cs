using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialCode.API.Requests.Posts;
using SocialCode.API.Services.Posts;

namespace SocialCode.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("posts")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpPost]
        public async Task<IActionResult> InsertPost([FromBody] PostRequest postRequest)
        {
            var postServiceResult = await _postService.Insert(postRequest);
            if (!postServiceResult.IsValid())
            {
                return ControllerUtils.ControllerUtils.TranslateErrorToResponseStatus(postServiceResult.ErrorTypes,
                    postServiceResult.ErrorMsg);
            }

            return new CreatedResult("/posts", postServiceResult.Value);
        }

        [HttpGet("{id:length(24)}")]
        public async Task<IActionResult> GetPostById(string id)
        {
            var postServiceResult = await _postService.GetPostById(id);
            if (!postServiceResult.IsValid())
            {
                return ControllerUtils.ControllerUtils.TranslateErrorToResponseStatus(postServiceResult.ErrorTypes,
                    postServiceResult.ErrorMsg);
            }

            return new OkObjectResult(postServiceResult.Value);
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> DeletePost(string id, [FromQuery]string userId)
        {
            var postServiceResult = await _postService.DeletePost(id, userId);

            if (!postServiceResult.IsValid())
            {
                return ControllerUtils.ControllerUtils.TranslateErrorToResponseStatus(postServiceResult.ErrorTypes,
                    postServiceResult.ErrorMsg);
            }

            return new OkObjectResult(postServiceResult.Value);
        }

        [HttpPut("edit/{id:length(24)}")]
        public async Task<IActionResult> ModifyPost([FromBody] PostRequest editedPost, string id, [FromQuery] string userId)
        {
            var postServiceResult = await _postService.ModifyPost(id, editedPost, userId);

            if (!postServiceResult.IsValid())
            {
                return ControllerUtils.ControllerUtils.TranslateErrorToResponseStatus(postServiceResult.ErrorTypes,
                    postServiceResult.ErrorMsg);
            }

            return new OkObjectResult(postServiceResult.Value);
        }

        [HttpGet("user/{userId:length(24)}")]
        public async Task<IActionResult> GetAllUserPosts(string userId)
        {
            var postServiceResult = await _postService.GetAllUserPosts(userId);
            if (!postServiceResult.IsValid())
            {
                return ControllerUtils.ControllerUtils.TranslateErrorToResponseStatus(postServiceResult.ErrorTypes,
                    postServiceResult.ErrorMsg);
            }

            return new OkObjectResult(postServiceResult.Value);

        }

        [HttpGet("recents")]
        public async Task<IActionResult> GetRecentPaginatedPost([FromQuery] int limit, [FromQuery] int offset)
        {
            var postServiceResult = await _postService.GetRecentPosts(limit, offset);
            if (!postServiceResult.IsValid())
            {
                return ControllerUtils.ControllerUtils.TranslateErrorToResponseStatus(postServiceResult.ErrorTypes,
                    postServiceResult.ErrorMsg);
            }

            return new OkObjectResult(postServiceResult.Value);
        }

        [HttpPost("getByTags")]
        public async Task<IActionResult> GetPaginatedPostsByTag([FromBody] TagFilters tagFilters,
            [FromQuery] int offset, [FromQuery] int limit)
        {
            var postServiceResult = await _postService.GetPostsByTags(tagFilters, limit, offset);
            if (!postServiceResult.IsValid())
            {
                return ControllerUtils.ControllerUtils.TranslateErrorToResponseStatus(postServiceResult.ErrorTypes,
                    postServiceResult.ErrorMsg);
            }

            return new OkObjectResult(postServiceResult.Value);
        }

        [HttpGet("saved/{userId:length(24)}")]
        public async Task<IActionResult> GetUserSavedPost(string userId)
        {
            var postServiceResult = await _postService.GetUserSavedPosts(userId);
            
            if (!postServiceResult.IsValid())
            {
                return ControllerUtils.ControllerUtils.TranslateErrorToResponseStatus(postServiceResult.ErrorTypes,
                    postServiceResult.ErrorMsg);
            }

            return new OkObjectResult(postServiceResult.Value);
        }

        [HttpPost("save")]
        public async Task<IActionResult> AddPostToUserSavedPosts([FromBody] SavePostRequest savePostRequest )
        {
            var postServiceResult = await _postService.AddPostToUserSavedPosts(savePostRequest);
            
            if (!postServiceResult.IsValid())
            {
                return ControllerUtils.ControllerUtils.TranslateErrorToResponseStatus(postServiceResult.ErrorTypes,
                    postServiceResult.ErrorMsg);
            }

            return new OkObjectResult(postServiceResult.Value);
        }

    }
}