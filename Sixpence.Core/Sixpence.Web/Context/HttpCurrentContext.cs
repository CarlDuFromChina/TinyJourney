using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sixpence.Web
{
    public class HttpCurrentContext
    {
        private static IHttpContextAccessor _accessor;
        public static HttpContext HttpContext => _accessor.HttpContext;
        public static HttpRequest Request => _accessor.HttpContext.Request;
        public static HttpResponse Response => _accessor.HttpContext.Response;
        internal static void Configure(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }
    }
}
