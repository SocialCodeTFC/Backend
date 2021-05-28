using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialCode.API.Requests.Comments;
using SocialCode.API.Services.Comments;

namespace SocialCode.API.Controllers
{ 
    [Authorize]
    [ApiController]
    [Route("comments")]
    public class CommentController
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService service)
        {
            _commentService = service;
        }
        
        [HttpPost]
        public async Task<IActionResult> InsertComment([FromBody] CommentRequest commentRequest)
        {
            var insertedComment = await _commentService.InsertComment(commentRequest);
            return new OkObjectResult(insertedComment);
            
        }
    }
}