using System.Threading.Tasks;
using SocialCode.API.Requests;
using SocialCode.API.Requests.Comments;


namespace SocialCode.API.Services.Comments
{
    public interface ICommentService
    {
        public  Task<SocialCodeResult<CommentResponse>> InsertComment(CommentRequest commentRequest);
        public  Task<SocialCodeResult<CommentResponse>> GetPostComments(CommentRequest commentRequest);
        public  Task<SocialCodeResult<CommentResponse>> GetCommentById(CommentRequest commentRequest);
        
    }
}