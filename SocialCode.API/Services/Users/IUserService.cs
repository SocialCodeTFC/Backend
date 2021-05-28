using System.Threading.Tasks;
using SocialCode.API.Requests;
using SocialCode.API.Requests.Users;

namespace SocialCode.API.Services.Users
{
    public interface IUserService
    {
        Task<SocialCodeResult<UserDataResponse>> GetUserById(string id);
        Task<SocialCodeResult<UserDataResponse>> DeleteUser(string id);
        Task<SocialCodeResult<UserDataResponse>> ModifyUserData(string id, UserDataRequest userDataRequest);
    }
}