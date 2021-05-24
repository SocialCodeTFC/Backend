using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialCode.API.Services.Requests.Users.Auth;
using SocialCode.API.Services.Requests.Users.Register;
using SocialCode.API.Services.Users;

namespace SocialCode.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id:length(24)}")]
        public async Task<IActionResult> GetOneById(string id)
        {
            var user = await _userService.GetUserById(id);
            
            return new OkObjectResult(user);
        }
        
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            var registeredUser = await _userService.Register(registerRequest);
            
            return new CreatedResult("/register",registeredUser);
        }
        
        [AllowAnonymous]
        [HttpPost("auth")]
        public async Task<IActionResult> Authenticate([FromBody] AuthRequest authRequest)
        {
            var authResponse = await _userService.Authenticate(authRequest);
            return new OkObjectResult(authResponse);

        }
        
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var deletedUser = await _userService.DeleteUser(id);
            return new OkObjectResult(deletedUser);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ModifyUser(string id, [FromBody] UserDataRequest updatedUserDataRequest)
        {
            var updatedUserResponse = await _userService.ModifyUserData(id, updatedUserDataRequest);
            return new OkObjectResult(updatedUserResponse);
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await _userService.GetCurrentUser();
            return new OkObjectResult(user);
        }
    }
}