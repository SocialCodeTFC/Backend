using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using SocialCode.Domain.User;
using SocialCode.Infrastructure.DbContext;


namespace SocialCode.Infrastructure.Repositories
{
    public class UserRepository :IUserRepository
    {
        private readonly IMongoDbContext _context;
        public UserRepository(IMongoDbContext context)
        {
            _context = context;
        }
        
        public async Task<User> Insert(User user)
        {
            var sameUsernameUser = await _context.Users.FindAsync(u => u.Username == user.Username);
            var sameEmailUser = await _context.Users.FindAsync(u => u.Email == user.Email);

            if (sameEmailUser.FirstOrDefault() is not null || sameUsernameUser.FirstOrDefault() is not null) return null;
            
            await _context.Users.InsertOneAsync(user);
            return user;
        }
        public async Task<User> GetUserById(string id)
        {
            var user = await _context.Users.FindAsync(e => e.Id == id);
            return await user.FirstOrDefaultAsync();
        }
        public async Task<User> DeleteUser(string id)
        {
            var user = await GetUserById(id);
            await _context.Users.DeleteOneAsync(x => x.Id == id);
            return user;
        }
        public async Task<User> ModifyUser(string id, User updatedUser)
        {
            try
            {
                await _context.Users.ReplaceOneAsync(x => x.Id == id, updatedUser,
                    new ReplaceOptions {IsUpsert = false});
            }
            catch (Exception)
            {
                return null;
            }
            
            return updatedUser;
        }
        public async Task<User> GetByUsername(string username)
        {
            var result = await _context.Users.FindAsync(x => x.Username == username);
            var user = await result.FirstOrDefaultAsync();
            return user;
        }
    }
}