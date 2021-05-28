using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialCode.API.Requests.Users;
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
    }
}