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
    public class PostController: ControllerBase
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
                return ControllerUtils.ControllerUtils.TranslateErrorToResponseStatus(postServiceResult.ErrorTypes, postServiceResult.ErrorMsg);
            }
            
            return new CreatedResult("/posts", postServiceResult.Value);
        }

        [HttpGet("{id:length(24)}")]
        public async Task<IActionResult> GetPostById(string id)
        {
            var postServiceResult = await _postService.GetPostById(id);
            if (!postServiceResult.IsValid())
            {
                return ControllerUtils.ControllerUtils.TranslateErrorToResponseStatus(postServiceResult.ErrorTypes, postServiceResult.ErrorMsg);
            }
            
            return new OkObjectResult(postServiceResult.Value);
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> DeletePost(string id)
        {
            var postServiceResult = await _postService.DeletePost(id);
            
            if (!postServiceResult.IsValid())
            {
                return ControllerUtils.ControllerUtils.TranslateErrorToResponseStatus(postServiceResult.ErrorTypes, postServiceResult.ErrorMsg);
            }

            return new OkObjectResult(postServiceResult.Value);        }

        [HttpPut("edit/{id:length(24)}")]
        public async Task<IActionResult> ModifyPost([FromBody] PostRequest editedPost, string id)
        {
            var postServiceResult = await _postService.ModifyPost(id, editedPost);
            
            if (!postServiceResult.IsValid())
            {
                return ControllerUtils.ControllerUtils.TranslateErrorToResponseStatus(postServiceResult.ErrorTypes, postServiceResult.ErrorMsg);
            }

            return new OkObjectResult(postServiceResult.Value);
        }
        
        [HttpGet("user/{userId:length(24)}")]
        public async Task<IActionResult> GetAllUserPosts(string userId)
        {
            var postServiceResult = await _postService.GetAllUserPosts(userId);
            if (!postServiceResult.IsValid())
            {
                return ControllerUtils.ControllerUtils.TranslateErrorToResponseStatus(postServiceResult.ErrorTypes, postServiceResult.ErrorMsg);
            }

            return new OkObjectResult(postServiceResult.Value);
            
        }

        [HttpGet("/paginated")]
        public async Task<IActionResult> GetPaginatedPost([FromQuery] int limit, [FromQuery] int offset)
        {
            //var paginatedPosts = await _postService.GetPaginatedPosts();
            return new OkObjectResult(null);
        }
        
    }
}