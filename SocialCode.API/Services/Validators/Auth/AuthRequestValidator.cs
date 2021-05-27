using SocialCode.API.Services.Requests.Users.Auth;
using SocialCode.API.Services.Requests.Users.Register;

namespace SocialCode.API.Services.Validators
{
    public static class AuthRequestValidator
    {
        public static bool IsValidLoginRequest(LoginRequest loginRequest)
        {
            return !(loginRequest is null) && loginRequest.Username.Length >= 2 && loginRequest.Username.Contains("#") && loginRequest.Password?.Length >= 5;
        }
        public static bool IsValidRegisterRequest(RegisterRequest registerRequest)
        {
            return registerRequest is { } && registerRequest.Email.Contains("@") && registerRequest.Password.Length > 4 && registerRequest.Password.Equals(registerRequest.RepeatPassword) && registerRequest.UserName.Contains("#");
        }
    }
}