using System.Threading.Tasks;
using SocialCode.API.Services.Requests.Users;
using SocialCode.API.Services.Requests.Users.Auth;
using SocialCode.API.Services.Requests.Users.Register;
using SocialCode.Domain.User;

namespace SocialCode.API.Services.Users
{
    public interface IUserService
    {
        Task<AuthResponse> Register(RegisterRequest registerRequest);
        Task<UserDataResponse> GetUserById(string id);
        Task<AuthResponse> Authenticate(AuthRequest authRequest);
        Task<UserDataResponse> DeleteUser(string id);
        Task<UserDataResponse> ModifyUserData(string id, UserDataRequest userDataRequest);
        Task<User> GetCurrentUser();
        Task<UserDataResponse> UpdateUser(string id, User user);
    }
}