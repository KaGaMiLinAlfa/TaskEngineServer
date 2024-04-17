using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Worker2.Comm
{
    
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CookieFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var cookie = context.HttpContext.Request.Cookies["LoginCookie"];
            if (string.IsNullOrEmpty(cookie))
                context.HttpContext.Response.Redirect("/User/Login");

            base.OnActionExecuting(context);
        }
    }
}