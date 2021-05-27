using System.Threading.Tasks;
using SocialCode.API.Services.Requests.Comments;


namespace SocialCode.API.Services.Comments
{
    public interface ICommentService
    {
        public  Task<CommentResponse> InsertComment(CommentRequest commentRequest);

        public Task<CommentResponse> GetCommentById(string commentId);
    }
}