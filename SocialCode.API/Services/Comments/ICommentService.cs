using System.Collections.Generic;
using System.Threading.Tasks;
using SocialCode.API.Requests;
using SocialCode.API.Requests.Comments;

namespace SocialCode.API.Services.Comments
{
    public interface ICommentService
    {
        Task<SocialCodeResult<CommentResponse>> InsertComment(CommentRequest commentRequest);
        Task<SocialCodeResult<IEnumerable<CommentResponse>>> GetCommentsByPostId(string postID);
        Task<SocialCodeResult<IEnumerable<CommentResponse>>> GetCommentsByUsername(string username);
    }
}