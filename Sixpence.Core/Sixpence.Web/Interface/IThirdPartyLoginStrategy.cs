using System;
using System.Collections.Generic;
using System.Text;
using Sixpence.Web.Model;

namespace Sixpence.Web
{
    /// <summary>
    /// 第三方联合登录接口
    /// </summary>
    public interface IThirdPartyLoginStrategy
    {
        /// <summary>
        /// 获取第三方登录方式名称
        /// </summary>
        /// <returns></returns>
        string GetName();

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="param">传入参数，大部分情况是一个 code</param>
        /// <returns></returns>
        LoginResponse Login(object param);
    }
}
