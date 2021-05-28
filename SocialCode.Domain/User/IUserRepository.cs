using System.Collections.Generic;
using System.Threading.Tasks;


namespace SocialCode.Domain.User
{
    public interface IUserRepository
    {
        Task<User> GetUserById(string id);
        Task<User> Insert(User user);
        Task<User> DeleteUser(string id);
        Task<User> ModifyUser(string id, User updatedUser);
        Task<User> GetByUsername(string username);
        Task<IEnumerable<User>> GetByLikedPost(string postId);
        Task<IEnumerable<User>> GetByPurchasedPost(string postId);
       
    }
}