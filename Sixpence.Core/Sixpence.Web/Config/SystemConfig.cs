﻿using Sixpence.Common.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sixpence.Web.Config
{
    /// <summary>
    /// 系统配置
    /// </summary>
    public class SystemConfig : BaseAppConfig<SystemConfig>
    {
        /// <summary>
        /// 默认密码
        /// </summary>
        public string DefaultPassword { get; set; }

        /// <summary>
        /// 域名
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// 协议
        /// </summary>
        public string Protocol { get; set; }

        /// <summary>
        /// 本地运行地址
        /// </summary>
        public string LocalUrls { get; set; }
        
        /// <summary>
        /// 日志备份天数
        /// </summary>
        public int LogBackupDays { get; set; }
    }
}
