using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using SocialCode.Domain.Post;
using SocialCode.Infrastructure.DbContext;


namespace SocialCode.Infrastructure.Repositories
{
    public class PostRepository: IPostRepository
    {
        private readonly IMongoDbContext _context;
        public PostRepository(IMongoDbContext context)
        {
            _context = context;
        }
        public async Task<Post> Insert(Post post)
        {
            await _context.Posts.InsertOneAsync(post);
            return post;
        }
        public async Task<Post> GetPostById(string id)
        {
            var post = await _context.Posts.FindAsync(p => p.Id == id);
            return await post.FirstOrDefaultAsync();
        }
        public async Task<Post> ModifyPost(Post updatedPost, string id)
        {
            var post = await _context.Posts.FindAsync(e => e.Id == id);
            if (await post.FirstOrDefaultAsync() is null) return null;
            var insertedPost = await _context.Posts.ReplaceOneAsync(e => e.Id == id, updatedPost);
            return insertedPost.IsAcknowledged ? updatedPost : null;
        }
        public async Task<Post> DeletePost(string id)
        {
            var deletePostResult = await _context.Posts.FindAsync(p => p.Id == id);
            
            var post = await deletePostResult.FirstOrDefaultAsync();
            
            if ( post is null) return null;
            
            post.IsDeleted = true;
            
            var deleteResult =  await _context.Posts.ReplaceOneAsync(p => p.Id == id, post);
            
            return deleteResult.IsAcknowledged ? post : null;
        }
        public async Task<IEnumerable<Post>> GetAllUserPosts(string userId)
        {
            var result = await _context.Posts.FindAsync(p => p.AuthorID == userId);
            var postsList = await result.ToListAsync();
            return postsList.Count <= 0 ? null : postsList;
        }
        public async Task<IEnumerable<Post>> GetRecentPosts(int limit, int offset)
        {
            var posts = await _context.Posts.FindAsync(_ => true);
            var postsList = await posts.ToListAsync();
            postsList.OrderByDescending(p => p.Timestamp).Take(limit-offset);
            
            return postsList ?? null;
        }
        public async Task<IEnumerable<Post>> GetPostByTagFilter(List<string> tags, int limit, int offset)
        {
            var posts = await _context.Posts.FindAsync(_ => true);
            var postsList = await posts.ToListAsync();
            var filteredList = new List<Post>();
            var listResult = new List<Post>();
            foreach (var tag in tags)
            {
                filteredList = postsList.FindAll(p => p.Tags.Contains(tag));
                if (filteredList.Count > 0)
                {
                    listResult.AddRange(filteredList);
                }
            }

            var test = filteredList.GroupBy(p => p.Id).Select(group => group.FirstOrDefault()).ToArray();
            Console.WriteLine(test.FirstOrDefault()?.Tags);
            
            return listResult;
        }
    }
}