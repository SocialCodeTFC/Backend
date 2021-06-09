using System.Net;
using Microsoft.AspNetCore.Mvc;
using SocialCode.API.Requests;

namespace SocialCode.API.Controllers.ControllerUtils
{
    public static class ControllerUtils
    {
        public static ActionResult TranslateErrorToResponseStatus(SocialCodeErrorTypes errorTypes, string errMsg )
        {
            return errorTypes switch
            {
                SocialCodeErrorTypes.BadRequest => new BadRequestObjectResult(errMsg),
                SocialCodeErrorTypes.Generic => Result(HttpStatusCode.InternalServerError, errMsg),
                SocialCodeErrorTypes.NotFound => new NotFoundObjectResult(errMsg),
                SocialCodeErrorTypes.Forbidden => Result(HttpStatusCode.Forbidden, errMsg),
                _ => new StatusCodeResult(501)
            };
        }
        
        private static ActionResult Result(HttpStatusCode statusCode, string reason) => new ContentResult
        {
            StatusCode = (int)statusCode,
            Content = $"Status Code: {(int)statusCode}; {statusCode}; {reason}",
            ContentType = "text/plain",
        };
    }
}