namespace SocialCode.API.Requests.Users.Auth
{
    public class AuthResponse
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        
        public string RefreshToken { get; set; }
    }
}