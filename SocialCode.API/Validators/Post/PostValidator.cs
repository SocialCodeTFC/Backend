using System.Linq;
using SocialCode.API.Requests.Posts;

namespace SocialCode.API.Validators.Post
{
    public static class PostValidator
    {
        public static bool IsValidPostRequest(PostRequest postRequest)
        {
            if (postRequest is null  ||   (!postRequest.IsFree && postRequest.Price.Equals(0)) || postRequest.Price < 0 || (postRequest.IsFree && !postRequest.Price.Equals(0))) return false;
            return true;
        }
    }
}