using Sixpence.Web.Auth;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using Sixpence.Common.Current;

namespace Sixpence.Web.WebApi
{
    public class WebApiContextFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var tokenHeader = context.HttpContext.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");
            var user = JwtHelper.SerializeJwt(tokenHeader);
            if (user != null)
            {
                UserIdentityUtil.SetCurrentUser(new CurrentUserModel() { Id = user.Uid, Code = user.Code, Name = user.Name});
            }
            else
            {
                UserIdentityUtil.SetCurrentUser(UserIdentityUtil.GetAnonymous());
            }
        }
    }
}
