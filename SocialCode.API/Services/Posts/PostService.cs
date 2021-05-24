using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SocialCode.API.Services.Converters;
using SocialCode.API.Services.Requests.Posts;
using SocialCode.API.Services.Users;
using SocialCode.Domain.Post;


namespace SocialCode.API.Services.Posts
{
    public class PostService :IPostService
    {

        private readonly IUserService _userService;
        private readonly IPostRepository _postRepository;

        public PostService(IPostRepository repository, IUserService userService)
        {
            _postRepository = repository;
            _userService = userService;
        }
        public async Task<PostResponse> Insert(PostRequest postRequest)
        {
            var post = PostConverter.PostRequest_ToPost(postRequest);
            
            post.Timestamp = DateTime.Now.ToString("g");
            var author = await _userService.GetCurrentUser();
            
            if (author is null) return null;
            
            post.AuthorID = author.Id;
            
            var insertedPost = await _postRepository.Insert(post);
            
            return insertedPost is null ? null : PostConverter.Post_ToPostResponse(post);
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