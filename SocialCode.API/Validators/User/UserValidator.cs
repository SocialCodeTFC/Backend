using SocialCode.API.Requests.Users;

namespace SocialCode.API.Validators.User
{
    public static class UserValidator
    {
        public static bool IsValidUserDataRequest(UserDataRequest userDataRequest)
        {
            if (string.IsNullOrWhiteSpace(userDataRequest.LastName) ||
                string.IsNullOrWhiteSpace(userDataRequest.FirstName) ||
                string.IsNullOrWhiteSpace(userDataRequest.UserName) ||
                string.IsNullOrWhiteSpace(userDataRequest.Email)) return false;

            return true;
        }
    }
}