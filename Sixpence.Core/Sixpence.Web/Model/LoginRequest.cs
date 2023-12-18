using System;
using System.Collections.Generic;
using System.Text;

namespace Sixpence.Web.Model
{
    public class LoginRequest
    {
        public string Code { get; set; }
        public string Password { get; set; }
        public string PublicKey { get; set; }
        public ThirdPartyLogin ThirdPartyLogin { get; set; }
    }

    /// <summary>
    /// 第三方登录参数
    /// </summary>
    public class ThirdPartyLogin
    {
        /// <summary>
        /// 第三方登录参数
        /// </summary>
        public object Param { get; set; }

        /// <summary>
        /// 第三方登录方式
        /// </summary>
        public string Type { get; set; }
    }
}
