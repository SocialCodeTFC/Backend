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
            if (!CommonValidator.IsValidId(postRequest.Author_Id)  )
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.BadRequest;
                scResult.ErrorMsg = "Invalid authorID in the request!";
                return scResult;
            }

            if (!PostValidator.isValidPostRequest(postRequest))
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
                return scResult;
            }

            var originalPost = await _postRepository.GetPostById(id);

            if (originalPost is null)
            {
                scResult.ErrorMsg = "Post not found!";
                scResult.ErrorTypes = SocialCodeErrorTypes.NotFound;
                return scResult;
            }
            
            var post = PostConverter.PostRequest_ToModifiedPost(postRequest, originalPost);

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
            
            var author = await _userRepository.GetUserById(post.AuthorID);
            if (author is null)
            {
                scResult.ErrorMsg = "There are some internal problems to recover post author, maybe it's deleted";
                scResult.ErrorTypes = SocialCodeErrorTypes.Generic;
            }
            
            if (CanReturnPost(post))
            {
                scResult.Value = PostConverter.Post_ToPostResponse(post);
                SetAuthorReferencesToPostResponse(scResult.Value, author);
                return scResult;
            }
            
            scResult.ErrorMsg = "Post has benn deleted";
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
                var result = await GetPostResponseListWithAuthorReferences(filteredPosts);
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
            
            var postResponseList = PostConverter.PostList_ToPostResponseList(filteredPostsList);
            
            scResult.Value = new PaginatedResult<PostResponse>()
            {
                Items = postResponseList
            };

            return scResult;
        }
        public async Task<SocialCodeResult<PaginatedResult<PostResponse>>> GetPostsByTags(TagFilters tagFilters, int limit, int offset)
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

            var filteredPostResponseList = PostConverter.PostList_ToPostResponseList(filteredPostList);
            
            scResult.Value = new PaginatedResult<PostResponse>
            {
                Items = filteredPostResponseList
            };

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
        private async Task<List<PostResponse>> GetPostResponseListWithAuthorReferences(IEnumerable<Post> posts)
        {
            var postResponseList = new List<PostResponse>();
            
            foreach (var post in posts)
            {
                var author = await _userRepository.GetUserById(post.AuthorID);
                
                if (author is null)
                {
                    return null;
                }
                
                var postResponse = PostConverter.Post_ToPostResponse(post);
                postResponse.AuthorName = author.FirstName;
                postResponse.AuthorUsername = author.Username;
                
                postResponseList.Add(postResponse);
            }

            return postResponseList;
        }
    }
}