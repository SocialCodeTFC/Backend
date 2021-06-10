using SocialCode.API.Requests.Users;

namespace SocialCode.API.Validators.User
{
    public static class UserDataRequestValidator
    {
        public static bool IsValidUserDataRequest(UserDataRequest userDataRequest)
        {
            if (string.IsNullOrWhiteSpace(userDataRequest.Username) ||
                string.IsNullOrWhiteSpace(userDataRequest.FirstName) ||
                string.IsNullOrWhiteSpace(userDataRequest.LastName) ||
                string.IsNullOrWhiteSpace(userDataRequest.Email))
                return false;

            return userDataRequest.Email.Contains("@") &&
                   userDataRequest.Username.Contains("@");
        }
    }
}