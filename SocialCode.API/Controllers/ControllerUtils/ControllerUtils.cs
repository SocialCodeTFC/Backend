using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialCode.API.Requests;
using SocialCode.API.Requests.Posts;

namespace SocialCode.API.Controllers.ControllerUtils
{
    public static class ControllerUtils
    {
        public static  ActionResult TranslatePostServiceResult(SocialCodeResult<PostResponse> serviceResult )
        {
            if (serviceResult.Value is null)
            {
                switch (serviceResult.ErrorTypes)
                {
                    case SocialCodeErrorTypes.BadRequest:
                        return new BadRequestObjectResult(serviceResult.ErrorMsg);
                    case SocialCodeErrorTypes.Generic:
                        return new StatusCodeResult(500);;
                    case SocialCodeErrorTypes.NotFound:
                        return new NotFoundObjectResult(serviceResult.ErrorMsg);
                    case SocialCodeErrorTypes.Forbidden:
                        return new ForbidResult(serviceResult.ErrorMsg);
                    case SocialCodeErrorTypes.InvalidOperation:
                        return new StatusCodeResult(409);
                    default:
                        return new StatusCodeResult(505);
                }
            }

            return new OkObjectResult(serviceResult.Value);
        }
    }
}