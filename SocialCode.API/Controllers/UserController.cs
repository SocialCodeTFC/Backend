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
            var userServiceResult = await _userService.GetUserById(id);
            
            if (!userServiceResult.IsValid())
            {
                return ControllerUtils.ControllerUtils.TranslateErrorToResponseStatus(userServiceResult.ErrorTypes,
                    userServiceResult.ErrorMsg);
            }

            return new OkObjectResult(userServiceResult.Value);
        }
        
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var userServiceResult = await _userService.DeleteUser(id);
            if (!userServiceResult.IsValid())
            {
                return ControllerUtils.ControllerUtils.TranslateErrorToResponseStatus(userServiceResult.ErrorTypes,
                    userServiceResult.ErrorMsg);
            }

            return new OkObjectResult(userServiceResult.Value);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> ModifyUser(string id, [FromBody] UserDataRequest updatedUserDataRequest)
        {
            var userServiceResult = await _userService.ModifyUserData(id, updatedUserDataRequest);
            if (!userServiceResult.IsValid())
            {
                return ControllerUtils.ControllerUtils.TranslateErrorToResponseStatus(userServiceResult.ErrorTypes,
                    userServiceResult.ErrorMsg);
            }

            return new OkObjectResult(userServiceResult.Value);
        }
    }
}