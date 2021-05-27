namespace SocialCode.API.Services.Requests.Users.Auth
{
    public class RefreshTokenRequest
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}