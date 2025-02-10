using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sixpence.Web.Auth;
using Sixpence.Web.Config;
using Sixpence.Web.Model;
using Sixpence.Web.Model.System;
using Sixpence.Web.Service;
using Sixpence.Web.WebApi;

namespace Sixpence.Web.Controllers
{
    public class SystemController : BaseApiController
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SystemService _systemService;
        public SystemController(IHttpContextAccessor contextAccessor, SystemService systemService)
        {
            _httpContextAccessor = contextAccessor;
            _systemService = systemService;
        }

        /// <summary>
        /// 获取公钥
        /// </summary>
        /// <returns></returns>
        [HttpGet("public_key"), AllowAnonymous]
        public string GetPublicKey()
        {
            return _systemService.GetPublicKey();
        }

        /// <summary>
        /// 获取随机图片
        /// </summary>
        /// <returns>图片源</returns>
        [HttpGet("random_image"), AllowAnonymous]
        public string GetRandomImage()
        {
            return _systemService.GetRandomImage();
        }

        /// <summary>
        /// 获取头像
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("avatar/{id}"), AllowAnonymous]
        public object GetAvatar(string id)
        {
            return _systemService.GetAvatar(id);
        }

        /// <summary>
        /// 测试
        /// </summary>
        /// <returns></returns>
        [HttpGet("test"), AllowAnonymous]
        public bool Test()
        {
            var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }
            else
            {
                return JwtHelper.SerializeJwt(token) != null;
            }
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("login"), AllowAnonymous]
        public LoginResponse Login(LoginRequest model)
        {
            return _systemService.Login(model);
        }

        /// <summary>
        /// 登录参数
        /// </summary>
        /// <returns></returns>
        [HttpGet("login_config"), AllowAnonymous]
        public object LoginConfig()
        {
            return Ok(SSOConfig.Config);
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("signup"), AllowAnonymous]
        public LoginResponse Signup(LoginRequest model)
        {
            UserIdentityUtil.SetCurrentUser(UserIdentityUtil.GetSystem());
            return _systemService.Signup(model);
        }

        /// <summary>
        /// 登录或注册
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("fast_signin"), AllowAnonymous]
        public LoginResponse SignInOrSignUp(LoginRequest model)
        {
            UserIdentityUtil.SetCurrentUser(UserIdentityUtil.GetSystem());
            return _systemService.SignInOrSignUp(model);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="password"></param>
        [HttpPut("password")]
        public void EditPassword([FromBody] string password)
        {
            _systemService.EditPassword(password);
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="id"></param>
        [HttpPut("password/reset")]
        public void ResetPassword(string id)
        {
            _systemService.ResetPassword(id);
        }

        /// <summary>
        /// 忘记密码
        /// </summary>
        /// <param name="code"></param>
        [HttpGet("password/forget")]
        public void ForgetPassword(string code)
        {
            _systemService.ForgetPassword(code);
        }

        /// <summary>
        /// 是否展示后台
        /// </summary>
        /// <returns></returns>
        [HttpGet("is_show_admin")]
        public bool GetShowAdmin()
        {
            return _systemService.GetShowAdmin();
        }
    }
}