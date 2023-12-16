using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sixpence.Web.Auth;

namespace Sixpence.Web.Model
{
    /// <summary>
    /// 登录返回结果
    /// </summary>
    public class LoginResponse
    {
        public LoginResponse() { }

        public LoginResponse(bool result, string message)
        {
            this.result = result;
            this.message = message;
            if (this.result)
            {
                level = LoginMesageLevel.Success.ToString();
            }
            else
            {
                level = LoginMesageLevel.Fail.ToString();
            }
        }

        public LoginResponse(bool result, string message, LoginMesageLevel level)
        {
            this.result = result;
            this.message = message;
            this.level = level.ToString();
        }

        /// <summary>
        /// 消息级别
        /// </summary>
        public string level { get; set; }

        /// <summary>
        /// 结果
        /// </summary>
        public bool result { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public string userId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// token
        /// </summary>
        public ComplexToken token { get; set; }

        /// <summary>
        /// 信息
        /// </summary>
        public string message { get; set; }
    }

    /// <summary>
    /// 登录消息级别
    /// </summary>
    public enum LoginMesageLevel
    {
        Success,
        Warning,
        Fail
    }
}