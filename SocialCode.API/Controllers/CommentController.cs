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
            var commentServiceResult = await _commentService.InsertComment(commentRequest);

            if (!commentServiceResult.IsValid())
                return ControllerUtils.ControllerUtils.TranslateErrorToResponseStatus(commentServiceResult.ErrorTypes,
                    commentServiceResult.ErrorMsg);

            return new CreatedResult("/comments", commentServiceResult.Value);
        }


        [HttpGet("{postId:length(24)}")]
        public async Task<IActionResult> GetCommentsByPostId(string postId)
        {
            var commentServiceResult = await _commentService.GetCommentsByPostId(postId);

            if (!commentServiceResult.IsValid())
                return ControllerUtils.ControllerUtils.TranslateErrorToResponseStatus(commentServiceResult.ErrorTypes,
                    commentServiceResult.ErrorMsg);
            return new OkObjectResult(commentServiceResult.Value);
        }
    }
}