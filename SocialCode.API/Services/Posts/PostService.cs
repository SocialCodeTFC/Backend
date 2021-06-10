using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SocialCode.API.Converters;
using SocialCode.API.Requests;
using SocialCode.API.Requests.Posts;
using SocialCode.API.Validators;
using SocialCode.API.Validators.Post;
using SocialCode.Domain.Post;
using SocialCode.Domain.User;

namespace SocialCode.API.Services.Posts
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;

        public PostService(IPostRepository repository, IUserRepository userRepository)
        {
            _postRepository = repository;
            _userRepository = userRepository;
        }

        public async Task<SocialCodeResult<PostResponse>> Insert(PostRequest postRequest)
        {
            var scResult = new SocialCodeResult<PostResponse>();
            
            if (!CommonValidator.IsValidId(postRequest.Author_Id))
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.BadRequest;
                scResult.ErrorMsg = "Invalid authorID in the request!";
                return scResult;
            }

            if (!PostRequestValidator.IsValidPostRequest(postRequest))
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.BadRequest;
                scResult.ErrorMsg = "Invalid body in the request!";
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
            SetAuthorReferencesToPostResponse(scResult.Value, author);

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
                var author = await _userRepository.GetUserById(post.AuthorID);

                if (author is null)
                {
                    scResult.ErrorMsg = "There are some internal problems to recover post author, maybe it's deleted";
                    scResult.ErrorTypes = SocialCodeErrorTypes.Generic;
                    return scResult;
                }

                scResult.Value = PostConverter.Post_ToPostResponse(post);
                SetAuthorReferencesToPostResponse(scResult.Value, author);

                return scResult;
            }

            scResult.ErrorMsg = "Post has been deleted!";
            scResult.ErrorTypes = SocialCodeErrorTypes.Forbidden;
            return scResult;
        }

        public async Task<SocialCodeResult<PostResponse>> DeletePost(string id, string userId)
        {
            var scResult = new SocialCodeResult<PostResponse>();

            if (!CommonValidator.IsValidId(id) || !CommonValidator.IsValidId(userId))
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

                if (!UserIsAuthor(post, userId))
                {
                    scResult.ErrorMsg = "Forbidden operation";
                    scResult.ErrorTypes = SocialCodeErrorTypes.Forbidden;
                    return scResult;
                }
                
                var author = await _userRepository.GetUserById(post.AuthorID);
                if (author is null)
                {
                    scResult.ErrorMsg = "There are some internal problems to recover post author, maybe it's deleted";
                    scResult.ErrorTypes = SocialCodeErrorTypes.Generic;
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
                SetAuthorReferencesToPostResponse(scResult.Value, author);
            }
            catch (Exception)
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.Generic;
            }

            return scResult;
        }

        public async Task<SocialCodeResult<PostResponse>> ModifyPost(string id, PostRequest postRequest, string userId)
        {
            var scResult = new SocialCodeResult<PostResponse>();

            if (!CommonValidator.IsValidId(id) || !CommonValidator.IsValidId(postRequest.Author_Id) || !CommonValidator.IsValidId(userId))
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.BadRequest;
                scResult.ErrorMsg = "Invalid ID request!";
                return scResult;
            }

            if (!PostRequestValidator.IsValidPostRequest(postRequest))
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.BadRequest;
                scResult.ErrorMsg = "Invalid updatedPostData in the request";
                return scResult;
            }
            
            var originalPost = await _postRepository.GetPostById(id);

            if (originalPost is null)
            {
                scResult.ErrorMsg = "Post not found!";
                scResult.ErrorTypes = SocialCodeErrorTypes.NotFound;
                return scResult;
            }
            if (!UserIsAuthor(originalPost, userId))
            {
                scResult.ErrorMsg = "Forbidden operation";
                scResult.ErrorTypes = SocialCodeErrorTypes.Forbidden;
                return scResult;
            }
            
            var post = PostConverter.PostRequest_ToModifiedPost(postRequest, originalPost);
            Post modifiedPost;
            try
            {
                modifiedPost = await _postRepository.ModifyPost(post, id);

                if (modifiedPost is null)
                {
                    scResult.ErrorTypes = SocialCodeErrorTypes.NotFound;
                    scResult.ErrorMsg = "Post not found";
                    return scResult;
                }
            }
            catch (Exception)
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.Generic;
                scResult.ErrorMsg = "Failed to modify post!";
                return scResult;
            }

            var author = await _userRepository.GetUserById(post.AuthorID);

            if (author is null)
            {
                scResult.ErrorMsg = "There are some internal problems to recover post author, maybe it's deleted";
                scResult.ErrorTypes = SocialCodeErrorTypes.Generic;
            }

            if (!author.Id.Equals(post.AuthorID))
            {
                scResult.ErrorMsg = "";
                scResult.ErrorTypes = SocialCodeErrorTypes.Generic;
                return scResult;
            }

            if (CanReturnPost(post))
            {
                scResult.Value = PostConverter.Post_ToPostResponse(post);
                SetAuthorReferencesToPostResponse(scResult.Value, author);
                
                return scResult;
            }

            scResult.ErrorMsg = "Post has been deleted";
            scResult.ErrorTypes = SocialCodeErrorTypes.Forbidden;
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

            var filteredPosts = RemoveDeletedPostsFromList(posts);

            if (CanReturnManyPosts(filteredPosts))
            {
                var result = await GetPostResponses_WithAuthorReferences(filteredPosts);
                if (result is null)
                {
                    scResult.ErrorMsg = "Failed to get post author reference";
                    scResult.ErrorTypes = SocialCodeErrorTypes.Generic;
                    return scResult;
                }

                scResult.Value = result;
                return scResult;
            }

            scResult.ErrorMsg = "Failed to retrieve posts!";
            return scResult;
        }

        public async Task<SocialCodeResult<PaginatedResult<PostResponse>>> GetRecentPosts(int limit, int offset)
        {
            var scResult = new SocialCodeResult<PaginatedResult<PostResponse>>();

            var postList = await _postRepository.GetRecentPosts(limit, offset);

            if (postList is null)
            {
                scResult.ErrorMsg = "Failed to get recent posts";
                scResult.ErrorTypes = SocialCodeErrorTypes.Generic;
                return scResult;
            }

            var filteredPostsList = RemoveDeletedPostsFromList(postList);

            if (!CanReturnManyPosts(filteredPostsList))
            {
                scResult.ErrorMsg = "Failed to filter deleted posts";
                scResult.ErrorTypes = SocialCodeErrorTypes.Generic;
                return scResult;
            }

            var postResponseList = await GetPostResponses_WithAuthorReferences(filteredPostsList);

            scResult.Value = new PaginatedResult<PostResponse>
            {
                Offset = offset,
                Limit = limit,
                Items = postResponseList
            };

            return scResult;
        }

        public async Task<SocialCodeResult<PaginatedResult<PostResponse>>> GetPostsByTags(TagFilters tagFilters,
            int limit, int offset)
        {
            var scResult = new SocialCodeResult<PaginatedResult<PostResponse>>();

            if (tagFilters.Tags is null || tagFilters is null)
            {
                scResult.ErrorMsg = "Bad tag filter in request!";
                scResult.ErrorTypes = SocialCodeErrorTypes.BadRequest;
                return scResult;
            }

            var postsWithMatchingTags = await _postRepository.GetPostByTagFilter(tagFilters.Tags, limit, offset);

            if (postsWithMatchingTags is null)
            {
                scResult.ErrorMsg = "Failed to retrieve posts by tags";
                scResult.ErrorTypes = SocialCodeErrorTypes.Generic;
                return scResult;
            }

            var filteredPostList = RemoveDeletedPostsFromList(postsWithMatchingTags);

            if (!CanReturnManyPosts(filteredPostList))
            {
                scResult.ErrorMsg = "Failed to filter deleted tags";
                scResult.ErrorTypes = SocialCodeErrorTypes.Generic;
                return scResult;
            }

            var result = await GetPostResponses_WithAuthorReferences(filteredPostList);

            scResult.Value = new PaginatedResult<PostResponse>
            {
                Offset = offset,
                Limit = limit,
                Items = result
            };

            return scResult;
        }
        
        public async Task<SocialCodeResult<IEnumerable<PostResponse>>> GetUserSavedPosts(string userId)
        {
            var scResult = new SocialCodeResult<IEnumerable<PostResponse>>();

            if (!CommonValidator.IsValidId(userId))
            {
                scResult.ErrorMsg = "Id is not valid";
                scResult.ErrorTypes = SocialCodeErrorTypes.BadRequest;
                return scResult;
            }

            var user = await _userRepository.GetUserById(userId);
            if (user is null)
            {
                scResult.ErrorMsg = "User not found";
                scResult.ErrorTypes = SocialCodeErrorTypes.NotFound;
                return scResult;
            }

            var userSavedPostsIds = user.SavedPostsIds;
            
            if (userSavedPostsIds is null || !userSavedPostsIds.Any())
            {
                scResult.Value = new List<PostResponse>();
                return scResult;
            }
            
            //Obtener en base a los ids de los posts la lista de posts
            var userSavedPosts = await _postRepository.GetPostsByIds(userSavedPostsIds);
            
            var filteredPostResponse = RemoveDeletedPostsFromList(userSavedPosts);
            
            if (!CanReturnManyPosts(filteredPostResponse))
            {
                scResult.ErrorMsg = "Failed to return user posts";
                scResult.ErrorTypes = SocialCodeErrorTypes.Generic;
                return scResult;
            }

            var postResponseList = await GetPostResponses_WithAuthorReferences(filteredPostResponse);

            scResult.Value = postResponseList;
            return scResult;
        }

        public async Task<SocialCodeResult<PostResponse>> AddPostToUserSavedPosts(SavePostRequest savePostRequest)
        {
            var scResult = new SocialCodeResult<PostResponse>();

            if (!CommonValidator.IsValidId(savePostRequest.UserId) || !CommonValidator.IsValidId(savePostRequest.PostId))
            {
                scResult.ErrorMsg = "Check UserID and PostID";
                scResult.ErrorTypes = SocialCodeErrorTypes.BadRequest;
                return scResult;
            }
            
            var user = await _userRepository.GetUserById(savePostRequest.UserId);
            var post = await _postRepository.GetPostById(savePostRequest.PostId);
            

            if (post is null)
            {
                scResult.ErrorMsg = "Post not found";
                scResult.ErrorTypes = SocialCodeErrorTypes.NotFound;
                return scResult;
            }
            
            var author = await _userRepository.GetUserById(post.AuthorID);

            if (user is null)
            {
                scResult.ErrorMsg = "User not found";
                scResult.ErrorTypes = SocialCodeErrorTypes.NotFound;
                return scResult;
            }

            if (user.SavedPostsIds is null)
            {
                user.SavedPostsIds = new List<string> {post.Id};
            }
            else
            {
                user.SavedPostsIds.Add(post.Id);   
            }
            
            var modifiedUser = await _userRepository.ModifyUser(savePostRequest.UserId, user);

            if (modifiedUser is null)
            {
                scResult.ErrorMsg = "Failed to add post to saved";
                scResult.ErrorTypes = SocialCodeErrorTypes.Generic;
                return scResult;
            }

            if (CanReturnPost(post))
            {
                var postResponse = PostConverter.Post_ToPostResponse(post);
                SetAuthorReferencesToPostResponse(postResponse, author);
                scResult.Value = postResponse;
                return scResult;
            }

            scResult.ErrorMsg = "Invalid Operation";
            scResult.ErrorTypes = SocialCodeErrorTypes.Forbidden;
            return scResult;
        }
        
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
            var filteredList = posts.Where(p => p.IsDeleted == false).ToList();

            return filteredList;
        }

        private static void SetAuthorReferencesToPostResponse(PostResponse postResponse, User postAuthor)
        {
            postResponse.AuthorName = postAuthor.FirstName;
            postResponse.AuthorUsername = postAuthor.Username;
        }

        private async Task<List<PostResponse>> GetPostResponses_WithAuthorReferences(IEnumerable<Post> posts)
        {
            var postResponseList = new List<PostResponse>();

            foreach (var post in posts)
            {
                var author = await _userRepository.GetUserById(post.AuthorID);

                if (author is null) return null;

                var postResponse = PostConverter.Post_ToPostResponse(post);
                postResponse.AuthorName = author.FirstName;
                postResponse.AuthorUsername = author.Username;

                postResponseList.Add(postResponse);
            }

            return postResponseList;
        }
        
        private bool UserIsAuthor(Post post, string userID)
        {
            return post is { } && userID.Equals(post.AuthorID);
        }
    }
}