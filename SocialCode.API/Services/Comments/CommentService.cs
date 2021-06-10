using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SocialCode.API.Converters;
using SocialCode.API.Requests;
using SocialCode.API.Requests.Comments;
using SocialCode.API.Validators;
using SocialCode.Domain.Comment;
using SocialCode.Domain.Post;
using SocialCode.Domain.User;

namespace SocialCode.API.Services.Comments
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;

        public CommentService(ICommentRepository repository, IUserRepository userRepository,
            IPostRepository postRepository)
        {
            _commentRepository = repository;
            _userRepository = userRepository;
            _postRepository = postRepository;
        }

        public async Task<SocialCodeResult<CommentResponse>> InsertComment(CommentRequest commentRequest)
        {
            var scResult = new SocialCodeResult<CommentResponse>();

            if (!CommonValidator.IsValidId(commentRequest?.PostId) || commentRequest?.Content is null)
            {
                scResult.ErrorMsg = "Invalid insert post request!";
                scResult.ErrorTypes = SocialCodeErrorTypes.BadRequest;
                return scResult;
            }

            var author = await _userRepository.GetUserByUsername(commentRequest.Username);
            var post = await _postRepository.GetPostById(commentRequest.PostId);

            if (author is null)
            {
                scResult.ErrorMsg = "Comment request author dont exist";
                scResult.ErrorTypes = SocialCodeErrorTypes.NotFound;
                return scResult;
            }

            if (post is null)
            {
                scResult.ErrorMsg = "The post reference doesn't exist in Db";
                scResult.ErrorTypes = SocialCodeErrorTypes.NotFound;
                return scResult;
            }

            var comment = CommentConverter.CommentRequest_ToComment(commentRequest);
            comment.Timestamp = DateTime.Now.ToString("g");
            comment.AuthorUsername = author.Username;
            comment.PostId = post.Id;

            var insertedComment = await _commentRepository.Insert(comment);

            if (insertedComment is null)
            {
                scResult.ErrorMsg = "Failed to save comment";
                scResult.ErrorTypes = SocialCodeErrorTypes.Generic;
                return scResult;
            }

            var commentResponse = CommentConverter.Comment_ToCommentResponse(insertedComment);
            scResult.Value = commentResponse;
            return scResult;
        }

        public async Task<SocialCodeResult<IEnumerable<CommentResponse>>> GetCommentsByPostId(string postId)
        {
            var scResult = new SocialCodeResult<IEnumerable<CommentResponse>>();

            if (!CommonValidator.IsValidId(postId))
            {
                scResult.ErrorMsg = "Post id is not valid!";
                scResult.ErrorTypes = SocialCodeErrorTypes.BadRequest;
                return scResult;
            }

            var comments = await _commentRepository.GetCommentByPostId(postId);

            if (comments is null)
            {
                scResult.ErrorMsg = "Post does not contains any comment!";
                scResult.ErrorTypes = SocialCodeErrorTypes.NotFound;
                return scResult;
            }

            scResult.Value = CommentConverter.CommentList_ToCommentResponseList(comments);


            return scResult;
        }

        public async Task<SocialCodeResult<IEnumerable<CommentResponse>>> GetCommentsByUsername(string username)
        {
            var scResult = new SocialCodeResult<IEnumerable<CommentResponse>>();

            if (username is null || !username.Contains("@"))
            {
                scResult.ErrorMsg = "InvalidUsername";
                scResult.ErrorTypes = SocialCodeErrorTypes.BadRequest;
                return scResult;
            }

            var author = await _userRepository.GetUserByUsername(username);
            
            if (author is null)
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.NotFound;
                scResult.ErrorMsg = "User not found";
                return scResult;
            }
            var comments = await _commentRepository.GetCommentsByUsername(username);

            if (comments is null)
            {
                scResult.ErrorMsg = "Failed to get comments";
                scResult.ErrorTypes = SocialCodeErrorTypes.Generic;
                return scResult;
            }

            scResult.Value = CommentConverter.CommentList_ToCommentResponseList(comments);
            return scResult;
        }
    }
}