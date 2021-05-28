namespace SocialCode.API.Requests.Users.Register
{
    public class RegisterRequest
    {
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        
        public string RepeatPassword { get; set; }
    }
}