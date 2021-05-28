using SocialCode.API.Requests.Users.Auth;
using SocialCode.API.Requests.Users.Register;

namespace SocialCode.API.Validators.Auth
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