using SocialCode.API.Requests.Users.Auth;
using SocialCode.API.Requests.Users.Register;

namespace SocialCode.API.Validators.Auth
{
    public static class AuthRequestValidator
    {
        public static bool IsValidLoginRequest(LoginRequest loginRequest)
        {
            if (string.IsNullOrWhiteSpace(loginRequest.Password) ||
                string.IsNullOrWhiteSpace(loginRequest.Username))
                return false;
            
            return !(loginRequest is null) && loginRequest.Username.Length >= 2 && loginRequest.Username.Contains("@") && loginRequest.Password?.Length >= 5;
        }
        public static bool IsValidRegisterRequest(RegisterRequest registerRequest)
        {
            
            return registerRequest.Email.Contains("@") && registerRequest.Password.Length >=  5 && registerRequest.Password.Equals(registerRequest.RepeatPassword) && registerRequest.UserName.Contains("@");
        }
    }
}