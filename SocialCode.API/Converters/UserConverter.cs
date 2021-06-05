using SocialCode.API.Requests.Users;
using SocialCode.API.Requests.Users.Auth;
using SocialCode.API.Requests.Users.Register;
using SocialCode.API.Services.Users;
using SocialCode.Domain.User;

namespace SocialCode.API.Converters
{
    public static class UserConverter
    {
        public static UserDataResponse User_ToUserResponse(User user)
        {
            return new UserDataResponse
            {
                Id = user?.Id,
                Email = user?.Email,
                FirstName = user?.FirstName,
                LastName = user?.LastName,
                Username = user?.Username
            };
        }

        public static User UserRequest_ToUser(UserDataRequest userDataRequest)
        {
            return new User()
            {
                Id = userDataRequest?.Id,
                Email = userDataRequest?.Email,
                Username = userDataRequest?.Username,
                FirstName = userDataRequest?.FirstName,
                LastName = userDataRequest?.LastName,
                Password = userDataRequest?.Password
            };
        }

        public static AuthResponse User_ToAuthResponse(User user)
        {
            return new AuthResponse
            {
                Id = user?.Id,
                Token = user?.Token,
                Username = user?.Username,
                RefreshToken = user?.RefreshToken
            };
        }
        
        public static User RegisterRequest_ToUser(RegisterRequest registerRequest)
        {
            return new User()
            {
                Username = registerRequest?.Username,
                Password = registerRequest?.Password,
                Email = registerRequest?.Email,
                FirstName = registerRequest?.FirstName,
                LastName = registerRequest?.LastName,

            };
        }
    }
}