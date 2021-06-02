using System;
using System.Threading.Tasks;
using SocialCode.API.Converters;
using SocialCode.API.Requests;
using SocialCode.API.Requests.Comments;
using SocialCode.API.Requests.Posts;
using SocialCode.API.Validators;
using SocialCode.Domain.Comment;
using SocialCode.Domain.Post;
using SocialCode.Domain.User;
using SocialCode.Infrastructure.Repositories;

namespace SocialCode.API.Services.Comments
{
    public class CommentService: ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPostRepository _postRepository;
        
        public CommentService(ICommentRepository repository, IUserRepository userRepository, IPostRepository postRepository)
        {
            _commentRepository = repository;
            _userRepository = userRepository;
            _postRepository = postRepository;
        }

        public async Task<SocialCodeResult<CommentResponse>> InsertComment(CommentRequest commentRequest)
        {
            var scResult = new SocialCodeResult<CommentResponse>();
                
            if (!CommonValidator.IsValidId(commentRequest?.AuthorId) || !CommonValidator.IsValidId(commentRequest?.PostId) || commentRequest?.Content is null)
            {
                scResult.ErrorMsg = "Invalid insert post request!";
                scResult.ErrorTypes = SocialCodeErrorTypes.BadRequest;
                return scResult;
            }
            
            //Comprobamos que existen tanto autor como post a los que referencia la request
            var commentAuthor = await _userRepository.GetUserById(commentRequest.AuthorId);
            var post = await _postRepository.GetPostById(commentRequest.PostId);

            if (commentAuthor is null)
            {
                scResult.ErrorMsg = "Comment request author dont exist";
                scResult.ErrorTypes = SocialCodeErrorTypes.NotFound;
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

            var commentResponse =  CommentConverter.Comment_ToCommentResponse(insertedComment);
            var updatedCommentResponse = SetUserReference(commentResponse , commentAuthor);
            scResult.Value = updatedCommentResponse;
            return scResult;
        }

        public Task<SocialCodeResult<CommentResponse>> GetPostComments(CommentRequest commentRequest)
        {
            throw new NotImplementedException();
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