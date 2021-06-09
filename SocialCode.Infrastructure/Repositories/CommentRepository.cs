using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using SocialCode.Domain.Comment;
using SocialCode.Infrastructure.DbContext;

namespace SocialCode.Infrastructure.Repositories
{
    public class CommentRepository :ICommentRepository
    {
        private readonly IMongoDbContext _context;

        public CommentRepository(IMongoDbContext context)
        {
            _context = context;
        }
        
        public async Task<Comment> GetCommentById(string id)
        {
            var result = await _context.Comments.FindAsync(c => c.Id == id);
            var comment = await result.FirstOrDefaultAsync();
            return comment ?? null;
        }
        public async Task<Comment> Insert(Comment comment)
        {
            await _context.Comments.InsertOneAsync(comment);
            return comment;
        }
        public async Task<Comment> DeleteComment(string id)
        {
            var commentResult = await _context.Comments.FindAsync(c => c.Id == id);
            var comment = await commentResult.FirstOrDefaultAsync();
            if (comment is null) return null;
            var deleteResult = await _context.Comments.DeleteOneAsync(c => c.Id == id);
            if (deleteResult.IsAcknowledged && !(comment is null)) return comment;
            return null;
        }
        public async Task<Comment> ModifyComment(Comment updatedComment, string id)
        {
            var insertResult = await _context.Comments.ReplaceOneAsync(c => c.Id == id, updatedComment);
            return !insertResult.IsAcknowledged ? null : updatedComment;
        }
        public async Task<IEnumerable<Comment>> GetCommentByPostId(string postId)
        {
            var result = await _context.Comments.FindAsync(c => c.PostId == postId);
            var comments = await result.ToListAsync();
            if (comments is null || comments.Count().Equals(0)) return null;
            return comments;
        }
    }
}