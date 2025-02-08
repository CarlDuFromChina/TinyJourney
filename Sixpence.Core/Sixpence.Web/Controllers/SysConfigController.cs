using Sixpence.Web.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sixpence.Web.Service;
using Sixpence.Web.Entity;

namespace Sixpence.Web.Controllers
{
    public class SysConfigController : EntityBaseController<SysConfig, SysConfigService>
    {
        public SysConfigController(SysConfigService service) : base(service)
        {
        }

        [HttpGet("value")]
        public object GetValue(string code)
        {
            return _service.GetValue(code);
        }

        /// <summary>
        /// 是否开启评论
        /// </summary>
        /// <returns></returns>
        [HttpGet("is_enable_comment"), AllowAnonymous]
        public string EnableComment()
        {
            return _service.GetValue("enable_comment")?.ToString();
        }

        /// <summary>
        /// 网站信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("website_info"), AllowAnonymous]
        public string WebsiteInfo()
        {
            return _service.GetValue("website_info")?.ToString();
        }
    }
}