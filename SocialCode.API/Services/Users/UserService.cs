using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SocialCode.API.Converters;
using SocialCode.API.Requests;
using SocialCode.API.Requests.Users;
using SocialCode.API.Validators;
using SocialCode.Domain.User;
namespace SocialCode.API.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository, IConfiguration config)
        {
            _userRepository = userRepository;
        }
        public async Task<SocialCodeResult<UserDataResponse>> GetUserById(string id)
        {
            var scResult = new SocialCodeResult<UserDataResponse>();
            
            if (!CommonValidator.IsValidId(id))
            {
                scResult.ErrorMsg = "ID is not valid" ;
                scResult.ErrorTypes = SocialCodeErrorTypes.BadRequest ;
                return scResult;
            }
            
            var user = await _userRepository.GetUserById(id);
            
            if (user is null)
            {
                scResult.ErrorMsg = "User not found";
                scResult.ErrorTypes = SocialCodeErrorTypes.NotFound;
                return scResult;
            }
            
            scResult.Value =  UserConverter.User_ToUserResponse(user);
            return scResult;
        }
        public async Task<SocialCodeResult<UserDataResponse>> DeleteUser(string id)
        {
            var scResult = new SocialCodeResult<UserDataResponse>();

            if (!CommonValidator.IsValidId(id))
            {
                scResult.ErrorMsg = "Invalid ID";
                scResult.ErrorTypes = SocialCodeErrorTypes.BadRequest;
                return scResult;
            }
            
            
            var user = await _userRepository.GetUserById(id);

            if (user is null)
            {
                scResult.ErrorMsg = "User not found";
                scResult.ErrorTypes = SocialCodeErrorTypes.NotFound;
                return scResult;
            }

            var deletedUser = await _userRepository.DeleteUser(id);

            if (deletedUser is null)
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.Generic;
                scResult.ErrorMsg = "Failed to delete user";
            }
            
            scResult.Value = UserConverter.User_ToUserResponse(deletedUser);
            return scResult;
        }
        public async Task<SocialCodeResult<UserDataResponse>> ModifyUserData(string id, UserDataRequest updatedUserDataRequest)
        {
            var scResult = new SocialCodeResult<UserDataResponse>();

            if (!CommonValidator.IsValidId(id))
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.BadRequest;
                scResult.ErrorMsg = "Invalid ID request";
                return scResult;
            }
            
            if (updatedUserDataRequest is null) //TODO: Change to validator function & correct validation
            {
                scResult.ErrorMsg = "Invalid update user data request";
                scResult.ErrorTypes = SocialCodeErrorTypes.BadRequest;
                return scResult;
            }
            
            var updatedUser = UserConverter.UserRequest_ToUser(updatedUserDataRequest);
            
            var updateResult = await _userRepository.ModifyUser(id, updatedUser);
            
            if (updateResult is null)
            {
                scResult.ErrorMsg = "Failed to update user data";
                scResult.ErrorTypes = SocialCodeErrorTypes.Generic;
                return scResult;
            }
            scResult.Value = UserConverter.User_ToUserResponse(updateResult);
            return scResult;
        }
    }
}