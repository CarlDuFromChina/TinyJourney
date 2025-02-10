using System;
using Sixpence.Common.Config;
using Sixpence.Web;

namespace Sixpence.TinyJourney.Config
{
    public class WebSiteInfoConfig : BaseAppConfig<WebSiteInfoConfig>
    {
        /// <summary>
        /// 站长
        /// </summary>
        public string Author { get; set; }
        
        /// <summary>
        /// 站长邮箱
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// 备案号
        /// </summary>
        public string RecordNo { get; set; }
        
        /// <summary>
        /// 主页用户
        /// </summary>
        public string IndexUser { get; set; }
        
        /// <summary>
        /// 小红书链接
        /// </summary>
        public string RedBookUrl { get; set; }
    }
}

