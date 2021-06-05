using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
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
            var authServiceResult = await _authService.Register(registerRequest);
            
            if (!authServiceResult.IsValid())
            {
                return ControllerUtils.ControllerUtils.TranslateErrorToResponseStatus(authServiceResult.ErrorTypes,
                    authServiceResult.ErrorMsg);
            }

            return new CreatedResult("/auth/register", authServiceResult.Value);
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var authServiceResult = await _authService.LogIn(loginRequest);
            
            if (!authServiceResult.IsValid())
            {
                return ControllerUtils.ControllerUtils.TranslateErrorToResponseStatus(authServiceResult.ErrorTypes,
                    authServiceResult.ErrorMsg);
            }

            return new OkObjectResult(authServiceResult.Value);

        }
        
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshUserToken([FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            var authServiceResult = await _authService.RefreshToken(refreshTokenRequest);
            
            if (!authServiceResult.IsValid())
            {
                return ControllerUtils.ControllerUtils.TranslateErrorToResponseStatus(authServiceResult.ErrorTypes,
                    authServiceResult.ErrorMsg);
            }

            return new OkObjectResult(authServiceResult.Value);
        }
    }
}