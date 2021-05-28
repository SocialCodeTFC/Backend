using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SocialCode.API.Requests.Users.Auth;
using SocialCode.API.Requests.Users.Register;
using SocialCode.API.Services.Auth;

namespace SocialCode.API.Controllers
{
    
    [ApiController]
    [Route("auth")]
    public class AuthController: ControllerBase
    {
        private readonly IAuthService _authService;
        
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            var registeredUser = await _authService.Register(registerRequest);
            
            return new CreatedResult("/auth/register",registeredUser);
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var scResult = await _authService.LogIn(loginRequest);
            
            return new OkObjectResult(scResult);

        }
        
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshUserToken([FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            var authResponse = await _authService.RefreshToken(refreshTokenRequest);
            return new OkObjectResult(authResponse);
        }
        
    }
}