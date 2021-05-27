using System;
using System.Threading.Tasks;
using SocialCode.API.Services.Converters;
using SocialCode.API.Services.Requests.Comments;
using SocialCode.Domain.Comment;

namespace SocialCode.API.Services.Comments
{
    public class CommentService: ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        
        public CommentService(ICommentRepository repository)
        {
            _commentRepository = repository;
        }

        public async Task<CommentResponse> InsertComment(CommentRequest commentRequest)
        {
            var comment = CommentConverter.CommentRequest_ToComment(commentRequest);
            comment.Timestamp = DateTime.Now.ToString("g");
            var result = await _commentRepository.Insert(comment);
            return CommentConverter.Comment_ToCommentResponse(comment) ?? null;
        }

        public Task<CommentResponse> GetCommentById(string commentId)
        {
            throw new NotImplementedException();
        }
    }
}