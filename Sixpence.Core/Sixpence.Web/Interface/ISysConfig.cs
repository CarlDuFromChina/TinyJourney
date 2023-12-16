using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Web;

namespace Sixpence.Web
{
    /// <summary>
    /// 系统配置
    /// </summary>
    public interface ISysConfig
    {
        /// <summary>
        /// 配置名
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 编码
        /// </summary>
        string Code { get; }

        /// <summary>
        /// 默认值
        /// </summary>
        object DefaultValue { get; }

        /// <summary>
        /// 备注描述
        /// </summary>
        string Description { get; }
    }
}