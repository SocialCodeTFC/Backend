using SocialCode.API.Requests.Auth;
using SocialCode.API.Requests.Users;
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