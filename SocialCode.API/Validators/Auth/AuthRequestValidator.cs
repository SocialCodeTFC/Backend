using SocialCode.API.Requests.Auth;

namespace SocialCode.API.Validators.Auth
{
    public static class AuthRequestValidator
    {
        public static bool IsValidLoginRequest(LoginRequest loginRequest)
        {
            if (string.IsNullOrWhiteSpace(loginRequest.Password) ||
                string.IsNullOrWhiteSpace(loginRequest.Username))
                return false;

            return !(loginRequest is null) && loginRequest.Username.Length >= 2 &&
                   loginRequest.Username.Contains("@") && loginRequest.Password?.Length >= 5;
        }

        public static bool IsValidRegisterRequest(RegisterRequest registerRequest)
        {
            if (string.IsNullOrWhiteSpace(registerRequest.Password) ||
                string.IsNullOrWhiteSpace(registerRequest.Username) ||
                string.IsNullOrWhiteSpace(registerRequest.FirstName) ||
                registerRequest.LastName is null ||
                string.IsNullOrWhiteSpace(registerRequest.Email) ||
                string.IsNullOrEmpty(registerRequest.RepeatPassword))
                return false;

            return registerRequest.Email.Contains("@") && registerRequest.Password.Length >= 5 &&
                   registerRequest.Password.Equals(registerRequest.RepeatPassword) &&
                   registerRequest.Username.Contains("@");
        }
    }
}