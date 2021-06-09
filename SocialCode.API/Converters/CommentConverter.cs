using System.Collections.Generic;
using System.Linq;
using SocialCode.API.Requests.Comments;
using SocialCode.Domain.Comment;
using SocialCode.Domain.User;

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
                AuthorUsername = commentRequest.Username,
                PostId = commentRequest.PostId
            };
        }
        public static CommentResponse Comment_ToCommentResponse(Comment comment)
        {
            if (comment is null) return null;

            return new CommentResponse
            {
                Content = comment.Content,
                Timestamp = comment.Timestamp,
                AuthorUsername = comment.AuthorUsername
            };
        }
        public static IEnumerable<CommentResponse> CommentList_ToCommentResponseList(IEnumerable<Comment> comments)
        {
            return comments?.Select(Comment_ToCommentResponse).ToList();
        }
    }
}