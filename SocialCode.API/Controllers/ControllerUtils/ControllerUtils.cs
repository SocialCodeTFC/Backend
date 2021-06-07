using System.Net;
using Microsoft.AspNetCore.Mvc;
using SocialCode.API.Requests;

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
                        return Result(HttpStatusCode.Forbidden, errMsg);
                    default:
                        return new StatusCodeResult(501);
                }
            }
        
        private static ActionResult Result(HttpStatusCode statusCode, string reason) => new ContentResult
        {
            StatusCode = (int)statusCode,
            Content = $"Status Code: {(int)statusCode}; {statusCode}; {reason}",
            ContentType = "text/plain",
        };
    }
}