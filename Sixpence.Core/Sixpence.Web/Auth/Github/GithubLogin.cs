using System;
using System.Collections.Generic;
using System.Text;
using Sixpence.Web.Service;
using Sixpence.Web.Model;
using Sixpence.Web.Entity;
using Sixpence.EntityFramework;
using Microsoft.Extensions.Logging;

namespace Sixpence.Web.Auth.Github
{
    public class GithubLogin : IThirdPartyLoginStrategy
    {
        private readonly IEntityManager _manager;
        private readonly GithubAuthService _githubAuthService;
        private readonly SysRoleService _sysRoleService;
        private readonly ILogger<GithubLogin> _logger;
        public GithubLogin(IEntityManager manager, ILogger<GithubLogin> logger, GithubAuthService githubAuthService, SysRoleService sysRoleService)
        {
            _manager = manager;
            _githubAuthService = githubAuthService;
            _sysRoleService = sysRoleService;
            _logger = logger;
        }
        public string GetName() => "Github";

        public LoginResponse Login(object param)
        {
            try
            {
                var code = param as string;
                var githubToken = _githubAuthService.GetAccessToken(code).Result;
                var githubUser = _githubAuthService.GetUserInfo(githubToken).Result;
                var user = _manager.QueryFirst<SysUser>(new { github_id = githubUser.id.ToString() });

                if (user != null)
                {
                    return new LoginResponse()
                    {
                        result = true,
                        userName = code,
                        token = JwtHelper.CreateToken(new JwtTokenModel() { Code = user.Code, Name = user.Name, Role = user.Code, Uid = user.Id }),
                        userId = user.Id,
                        message = "登录成功"
                    };
                }

                return _manager.ExecuteTransaction(() =>
                {
                    var role = _sysRoleService.GetGuest();
                    var id = Guid.NewGuid().ToString();
                    var avatarId = _githubAuthService.DownloadImage(githubUser.avatar_url, id).Result;
                    var user = new SysUser()
                    {
                        Id = id,
                        Code = githubUser.id.ToString(),
                        Password = null,
                        Name = githubUser.name,
                        Mailbox = githubUser.email,
                        Introduction = githubUser.bio,
                        Avatar = avatarId,
                        RoleId = role.Id,
                        RoleName = role.Name,
                        GithubId = githubUser.id.ToString(),
                        isActive = true,
                    };
                    _manager.Create(user, false);
                    var _authUser = new SysAuthUser()
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Code = user.Code,
                        RoleId = user.RoleId,
                        RoleName = user.RoleName,
                        UserId = user.Id,
                        IsLock = false,
                        LastLoginTime = DateTime.Now,
                        Password = null
                    };
                    _manager.Create(_authUser);

                    return new LoginResponse()
                    {
                        result = true,
                        userName = user.Name,
                        token = JwtHelper.CreateToken(new JwtTokenModel() { Code = _authUser.Code, Name = _authUser.Name, Role = _authUser.Code, Uid = _authUser.Id }),
                        userId = _authUser.UserId,
                        message = "登录成功"
                    };
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Github 登录失败：" + ex.Message, ex);
                _manager.Dispose();
                return new LoginResponse() { result = false, message = "Github 登录失败" };
            }
        }
    }
}
