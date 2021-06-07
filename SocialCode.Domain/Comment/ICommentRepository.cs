using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialCode.Domain.Comment
{
    public interface ICommentRepository
    {
        Task<Comment> GetCommentById(string id);
        Task<Comment> Insert(Comment comment);
        Task<Comment> DeleteComment(string id);
        Task<Comment> ModifyComment(Comment updatedComment, string id);
        Task<IEnumerable<Comment>> GetManyCommentGetCommentsByIds(IEnumerable<string> postIds);
    }
}