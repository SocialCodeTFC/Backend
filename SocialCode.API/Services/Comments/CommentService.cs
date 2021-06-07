using System;
using System.Collections.Generic;
using System.Linq;
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

            if (!CommonValidator.IsValidId(commentRequest?.AuthorId) ||
                !CommonValidator.IsValidId(commentRequest?.PostId) || commentRequest?.Content is null)
            {
                scResult.ErrorMsg = "Invalid insert post request!";
                scResult.ErrorTypes = SocialCodeErrorTypes.BadRequest;
                return scResult;
            }

            //Comprobamos que existen tanto autor como post a los que referencia la request
            var author = await _userRepository.GetUserById(commentRequest.AuthorId);
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


            var insertedComment = await _commentRepository.Insert(comment);

            if (insertedComment is null)
            {
                scResult.ErrorMsg = "Failed to save comment";
                scResult.ErrorTypes = SocialCodeErrorTypes.Generic;
                return scResult;
            }

            if (post.CommentIds is null)
                post.CommentIds = new List<string> {comment.Id};
            else
                post.CommentIds.Add(comment.Id);


            var updatedPost = await _postRepository.ModifyPost(post, post.Id);
            if (updatedPost is null)
            {
                scResult.ErrorMsg = "Failed updating post comment list";
                scResult.ErrorTypes = SocialCodeErrorTypes.Generic;
                return scResult;
            }

            var commentResponse = CommentConverter.Comment_ToCommentResponse(insertedComment);
            var updatedCommentResponse = SetUserReference(commentResponse, author);
            scResult.Value = updatedCommentResponse;
            return scResult;
        }

        public async Task<IEnumerable<CommentResponse>> GetManyCommentsByIds(IList<string> commentsIds)
        {
            var comments = await _commentRepository.GetManyCommentGetCommentsByIds(commentsIds.ToList());
            return comments is null ? null : comments.Select(CommentConverter.Comment_ToCommentResponse).ToList();
        }

        public Task<SocialCodeResult<CommentResponse>> GetCommentById(CommentRequest commentRequest)
        {
            throw new NotImplementedException();
        }
        
        private static CommentResponse SetUserReference(CommentResponse commentResponse, User author)
        {
            commentResponse.AuthorUsername = author.Username;
            return commentResponse;
        }
    }
}