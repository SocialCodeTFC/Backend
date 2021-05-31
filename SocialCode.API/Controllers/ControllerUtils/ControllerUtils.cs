using Microsoft.AspNetCore.Mvc;
using SocialCode.API.Requests;
using SocialCode.API.Requests.Posts;

namespace SocialCode.API.Controllers.ControllerUtils
{
    public static class ControllerUtils
    {
        public static ActionResult TranslateErrorToResponseStatus(SocialCodeErrorTypes errorTypes, string errMsg )
        {
                switch (errorTypes)
                {
                    case SocialCodeErrorTypes.BadRequest:
                        return new BadRequestObjectResult(errMsg);
                    case SocialCodeErrorTypes.Generic:
                        return new StatusCodeResult(500);;
                    case SocialCodeErrorTypes.NotFound:
                        return new NotFoundObjectResult(errMsg);
                    case SocialCodeErrorTypes.Forbidden:
                        return new UnauthorizedObjectResult(errMsg);
                    default:
                        return new StatusCodeResult(501);
                }
            }
    }
}