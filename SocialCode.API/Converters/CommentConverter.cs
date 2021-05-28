using SocialCode.API.Requests.Comments;
using SocialCode.Domain.Comment;

namespace SocialCode.API.Converters
{
    public static class CommentConverter
    {
        public static Comment CommentRequest_ToComment(CommentRequest commentRequest)
        {
            if (commentRequest is null) return null;
            
            return new Comment
            {
                Content = commentRequest.Content,
                AuthorId = commentRequest.AuthorId,
                PostId = commentRequest.PostId
            };
        }

        public static CommentResponse Comment_ToCommentResponse(Comment comment)
        {
            if (comment is null) return null;

            return new CommentResponse
            {
                Content = comment.Content,
                Timestamp = comment.Timestamp
            };
        }
    }
}