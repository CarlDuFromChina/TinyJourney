using Sixpence.Web.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sixpence.Web.Service;
using Sixpence.Web.Entity;

namespace Sixpence.Web.Controllers
{
    public class SysConfigController : EntityBaseController<SysConfig, SysConfigService>
    {
        [HttpGet("value")]
        public object GetValue(string code)
        {
            return new SysConfigService().GetValue(code);
        }

        /// <summary>
        /// 是否开启评论
        /// </summary>
        /// <returns></returns>
        [HttpGet("is_enable_comment"), AllowAnonymous]
        public string EnableComment()
        {
            return new SysConfigService().GetValue("enable_comment")?.ToString();
        }

        /// <summary>
        /// 网站信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("website_info"), AllowAnonymous]
        public string WebsiteInfo()
        {
            return new SysConfigService().GetValue("website_info")?.ToString();
        }
    }
}