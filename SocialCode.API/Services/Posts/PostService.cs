using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SocialCode.API.Converters;
using SocialCode.API.Requests;
using SocialCode.API.Requests.Posts;
using SocialCode.API.Services.Users;
using SocialCode.API.Validators;
using SocialCode.API.Validators.Post;
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
            if (!CommonValidator.IsValidId(postRequest.Author_Id))
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.BadRequest;
                scResult.ErrorMsg = "Invalid authorID in the request!";
                return scResult;
            }
            
            var post = PostConverter.PostRequest_ToPost(postRequest);
            
            var author = await _userRepository.GetUserById(postRequest.Author_Id);

            if (author is null)
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.BadRequest;
                scResult.ErrorMsg = "User ID doesn't match with any DB ID";
                return scResult;
            }
            
            post.AuthorID = author.Id;
            post.Timestamp = DateTime.Now.ToString("g");
            
            var insertedPost = await _postRepository.Insert(post);

            if (insertedPost is null)
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.BadRequest;
                scResult.ErrorMsg = "Failed to save Post";
                return scResult;
            }
            scResult.Value = PostConverter.Post_ToPostResponse(post);
            return scResult;
        }

        public async Task<SocialCodeResult<PostResponse>> GetPostById(string id)
        {
            var scResult = new SocialCodeResult<PostResponse>();

            if (!CommonValidator.IsValidId(id))
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.BadRequest;
                scResult.ErrorMsg = "Invalid ID request!";
                return scResult;
            }
            
            var post = await _postRepository.GetPostById(id);
            
            if (post is null)
            {
                scResult.ErrorMsg = "Post not found";
                scResult.ErrorTypes = SocialCodeErrorTypes.NotFound;
                return scResult;
            }

            if (CanReturnPost(post))
            {
                scResult.Value = PostConverter.Post_ToPostResponse(post);
                return scResult;
            }

            scResult.ErrorMsg = "Post is deleted!";
            scResult.ErrorTypes = SocialCodeErrorTypes.InvalidOperation;
            return scResult;
        }

        public async Task<SocialCodeResult<PostResponse>> DeletePost(string id)
        {
            var scResult = new SocialCodeResult<PostResponse>();
            
            if (!CommonValidator.IsValidId(id))
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.BadRequest;
                scResult.ErrorMsg = "Invalid ID request!";
                return scResult;
            }

            try
            {
                var post = await _postRepository.GetPostById(id);
                if (post is null)
                {
                    scResult.ErrorMsg = "Post not found";
                    scResult.ErrorTypes = SocialCodeErrorTypes.NotFound;
                    return scResult;      
                }
                
                
                //Delete PostReferenceOnUsers & deletePost
                
                var deletedPost = await _postRepository.DeletePost(id);
                
                if (deletedPost is null)
                {
                    scResult.ErrorTypes = SocialCodeErrorTypes.BadRequest;
                    scResult.ErrorMsg = "Failed to delete post";
                    return scResult;
                }
                
                scResult.Value = PostConverter.Post_ToPostResponse(deletedPost);
                
            }
            catch (Exception e)
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.Generic;
            }
            return scResult;
        }

        public async Task<SocialCodeResult<PostResponse>> ModifyPost(string id, PostRequest postRequest)
        {
            var scResult = new SocialCodeResult<PostResponse>();
            
            if (!CommonValidator.IsValidId(id) || !CommonValidator.IsValidId(postRequest.Author_Id))
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.BadRequest;
                scResult.ErrorMsg = "Invalid ID request!";
                return scResult;
            }
            
            if (!PostValidator.isValidPostRequest(postRequest))
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.BadRequest;
                scResult.ErrorMsg = "Invalid updatedPostData in the request";
            }
            
            var post = PostConverter.PostRequest_ToPost(postRequest);

            try
            {
                var modifiedPost = await _postRepository.ModifyPost(post, id);
                
                if (modifiedPost is null)
                {
                    scResult.ErrorTypes = SocialCodeErrorTypes.NotFound;
                    scResult.ErrorMsg = "Post not found";
                    return scResult;
                }
            }
            catch (Exception e)
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.Generic;
                scResult.ErrorMsg = "Failed to modify post!";
                return scResult;
            }

            if (CanReturnPost(post))
            {
                scResult.Value = PostConverter.Post_ToPostResponse(post);
                return scResult;
            }

            scResult.ErrorMsg = "Post has benn deleted";
            scResult.ErrorTypes = SocialCodeErrorTypes.InvalidOperation;
            return scResult;
            
        }

        public async Task<SocialCodeResult<IEnumerable<PostResponse>>> GetAllUserPosts(string userId)
        {
            var scResult = new SocialCodeResult<IEnumerable<PostResponse>>();
            
            if (!CommonValidator.IsValidId(userId))
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.BadRequest;
                scResult.ErrorMsg = "Invalid userID in the request!";
                return scResult;
            }
            
            var posts = await _postRepository.GetAllUserPosts(userId);
            if (posts is null)
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.NotFound;
                scResult.ErrorMsg = "Any post found";
                return scResult;
            }

            posts = RemoveDeletedPostsFromList(posts);
            
            if (CanReturnManyPosts(posts))
            {
                scResult.Value = PostConverter.PostList_ToPostResponseList(posts);
                return scResult;
            }

            scResult.ErrorMsg = "Any";
            return scResult;
        }

        //PageResult with latestPosts
        
        //PageResult with followedUser latest posts
        private static bool CanReturnPost(Post post)
        {
            return !post.IsDeleted;
        }
        private static bool CanReturnManyPosts(IEnumerable<Post> posts)
        {
            
            
            return posts.All(post => !(post.IsDeleted is true));
        }
        private static IEnumerable<Post> RemoveDeletedPostsFromList(IEnumerable<Post> posts)
        {
            foreach (var post in posts)
            {
                if (post.IsDeleted is true)
                {
                    posts.ToList().Remove(post);
                }
            }

            return posts;
        }
        
    }
}