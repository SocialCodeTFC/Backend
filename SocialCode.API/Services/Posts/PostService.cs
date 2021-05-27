using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SocialCode.API.Services.Converters;
using SocialCode.API.Services.Requests;
using SocialCode.API.Services.Requests.Posts;
using SocialCode.API.Services.Users;
using SocialCode.Domain.Post;
using SocialCode.Domain.User;


namespace SocialCode.API.Services.Posts
{
    public class PostService :IPostService
    {

        private readonly IUserRepository _userRepository;
        private readonly IPostRepository _postRepository;

        public PostService(IPostRepository repository, IUserRepository userRepository)
        {
            _postRepository = repository;
            _userRepository = userRepository;
        }
        
        public async Task<SocialCodeResult<PostResponse>> Insert(PostRequest postRequest)
        {
            var scResult = new SocialCodeResult<PostResponse>();
            
            //Validate postRequest with post RequestValidator
            
            var post = PostConverter.PostRequest_ToPost(postRequest);
            
            var author = await _userRepository.GetUserById(postRequest.Author_Id);

            if (author is null)
            {
                scResult.Error = SocialCodeError.BadRequest;
                scResult.ErrorMsg = "User ID doesn't match with any DB ID";
                return scResult;
            }
            
            post.AuthorID = author.Id;
            post.Timestamp = DateTime.Now.ToString("g");
            
            var insertedPost = await _postRepository.Insert(post);

            if (insertedPost is null)
            {
                scResult.Error = SocialCodeError.BadRequest;
                scResult.ErrorMsg = "Failed to save Post";
                return scResult;
            }
            scResult.Value = PostConverter.Post_ToPostResponse(post);
            return scResult;
        }

        public async Task<PostResponse> GetPostById(string id)
        {
            var post = await _postRepository.GetPostById(id);
            return post is null ? null : PostConverter.Post_ToPostResponse(post);
        }

        public async Task<PostResponse> DeletePost(string id)
        {
            var deletedPost = await _postRepository.DeletePost(id);
            return PostConverter.Post_ToPostResponse(deletedPost);
        }

        public async Task<PostResponse> ModifyPost(string id, PostRequest postRequest)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<PostResponse>> GetAllUserPosts(string userId)
        {
            var posts = await _postRepository.GetAllUserPosts(userId);
            return posts is null ? null : PostConverter.PostList_ToPostResponseList(posts);
        }
    }
}