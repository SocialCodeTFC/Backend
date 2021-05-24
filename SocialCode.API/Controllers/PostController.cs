using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialCode.API.Services.Posts;
using SocialCode.API.Services.Requests.Posts;

namespace SocialCode.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("posts")]
    public class PostController
    {
        private readonly IPostService _postService;
        
        public PostController(IPostService postService)
        {
            _postService = postService;
        }
        
        [HttpPost]
        public async Task<IActionResult> InsertPost([FromBody] PostRequest postRequest)
        {
            var post = await _postService.Insert(postRequest);
            return new OkObjectResult(post);
        }

        [HttpGet("{id:length(24)}")]
        public async Task<IActionResult> GetPostById(string id)
        {
            var post = await _postService.GetPostById(id);
            return new OkObjectResult(post);
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> DeletePost(string id)
        {
            var deletedPost = await _postService.DeletePost(id);
            return new OkObjectResult(deletedPost);
        }

        [HttpPut("edit/{id:length(24)}")]
        public async Task<IActionResult> EditPost([FromBody] PostRequest editedPost, string id)
        {
            var modifiedPost = await _postService.ModifyPost(id, editedPost);
            return new ObjectResult(modifiedPost);
        }
        
        [HttpGet("user/{userId:length(24)}")]
        public async Task<IActionResult> GetAllUserPosts(string userId)
        {
            var posts = await _postService.GetAllUserPosts(userId);
            return new OkObjectResult(posts);
        }
    }
}