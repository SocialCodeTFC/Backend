using System.Collections.Generic;
using System.Threading.Tasks;
using SocialCode.API.Requests;
using SocialCode.API.Requests.Comments;


namespace SocialCode.API.Services.Comments
{
    public interface ICommentService
    {
        public  Task<SocialCodeResult<CommentResponse>> InsertComment(CommentRequest commentRequest);
        public  Task<SocialCodeResult<IEnumerable<CommentResponse>>> GetCommentsByPostId(string postID);
        public  Task<IEnumerable<CommentResponse>> GetManyCommentsByIds(IList<string> commentsIds);
        
        
    }
}