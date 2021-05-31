using System.Linq;
using SocialCode.API.Requests.Posts;

namespace SocialCode.API.Validators.Post
{
    public static class PostValidator
    {
        public static bool isValidPostRequest(PostRequest postRequest)
        {
            if (postRequest is null) return false;
            var postAttributes = postRequest.GetType().GetProperties().ToList();
            
            foreach (var attribute in postAttributes)
            {
                if (attribute.GetValue(postRequest) is null) return false;
            }
            return true;
        }
    }
}