﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Authentication;
using System.Text;
using Sixpence.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Sixpence.Web.WebApi
{
    public class WebApiExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger<WebApiExceptionFilter> _logger;
        public WebApiExceptionFilter(IServiceProvider provider)
        {
            _logger = provider.GetService<ILoggerFactory>().CreateLogger<WebApiExceptionFilter>();
        }

        // 重写基类的异常处理方法
        public override void OnException(ExceptionContext context)
        {
            var exception = context.Exception.GetBaseException();

            // 统一处理报错信息
            if (exception is TimeoutException)
            {
                ContentResult result = new ContentResult
                {
                    StatusCode = 408,
                    ContentType = "application/json; charset=utf-8",
                    Content = "系统请求超时"
                };
                context.Result = result;
                context.ExceptionHandled = true;
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
            }
            else if (exception is FileNotFoundException)
            {
                ContentResult result = new ContentResult
                {
                    StatusCode = 404,
                    ContentType = "application/json; charset=utf-8",
                    Content = "访问资源未找到"
                };
                context.Result = result;
                context.ExceptionHandled = true;
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            else if (exception is InvalidCredentialException)
            {
                ContentResult result = new ContentResult
                {
                    StatusCode = 403,
                    ContentType = "application/json; charset=utf-8",
                    Content = context.Exception.Message
                };
                context.Result = result;
                context.ExceptionHandled = true;
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            }
            else if (exception is SpException)
            {
                ContentResult result = new ContentResult
                {
                    StatusCode = 500,
                    ContentType = "application/json; charset=utf-8",
                    Content = context.Exception.Message
                };
                context.Result = result;
                context.ExceptionHandled = true;
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            else
            {
                ContentResult result = new ContentResult
                {
                    StatusCode = 500,
                    ContentType = "application/json; charset=utf-8",
                    Content = "系统异常，请联系管理员"
                };
                context.Result = result;
                context.ExceptionHandled = true;
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                _logger.LogError(exception, exception.Message);
            }

            base.OnException(context);
        }
    }
}
