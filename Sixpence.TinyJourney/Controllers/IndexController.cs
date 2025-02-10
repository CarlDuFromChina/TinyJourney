using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sixpence.TinyJourney.Config;
using Sixpence.Web.Service;
using Sixpence.Web.WebApi;

namespace Sixpence.TinyJourney.Controller
{
    public class IndexController : BaseApiController
    {
        private readonly SysUserService _userService;
        public IndexController(SysUserService userService)
        {
            _userService = userService;
        }
        
        /// <summary>
        /// 获取网站信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("website_info"), AllowAnonymous]
        public object GetWebsiteInfo()
        {
            return Ok(WebSiteInfoConfig.Config);
        }

        /// <summary>
        /// 获取主页用户
        /// </summary>
        /// <returns></returns>
        [HttpGet("owner"), AllowAnonymous]
        public object GetOwnerUserInfo()
        {
            if (!string.IsNullOrEmpty(WebSiteInfoConfig.Config.IndexUser))
            {
                return Ok(_userService.GetDataByCode(WebSiteInfoConfig.Config.IndexUser));
            }

            return Ok();
        }
    }
}