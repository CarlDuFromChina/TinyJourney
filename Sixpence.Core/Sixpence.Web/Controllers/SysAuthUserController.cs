using Sixpence.Web.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sixpence.Web.Entity;
using Sixpence.Web.Auth;
using Sixpence.Web.Service;

namespace Sixpence.Web.Controllers
{
    public class SysAuthUserController : EntityBaseController<SysAuthUser, SysAuthUserService>
    {
        public SysAuthUserController(SysAuthUserService service) : base(service)
        {
        }

        /// <summary>
        /// 刷新Token
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize(Policy = "Refresh")]
        [Route("refresh_access_token")]
        public Token RefreshAccessToken()
        {
            var tokenHeader = HttpContext.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");
            var user = JwtHelper.SerializeJwt(tokenHeader);
            return JwtHelper.CreateAccessToken(user);
        }

        /// <summary>
        /// 锁定用户
        /// </summary>
        /// <param name="id"></param>
        [HttpPut("{id}/lock")]
        public void LockUser(string id)
        {
            _service.LockUser(id);
        }

        /// <summary>
        /// 解锁用户
        /// </summary>
        /// <param name="id"></param>
        [HttpPut("{id}/unlock")]
        public void UnlockUser(string id)
        {
            _service.UnlockUser(id);
        }

        /// <summary>
        /// 绑定第三方系统 账号
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="code"></param>
        [HttpPost("bind")]
        public void BindThirdPartyAccount(string type, string userid, string code)
        {
            _service.BindThirdPartyAccount(type, userid, code);
        }
    }
}