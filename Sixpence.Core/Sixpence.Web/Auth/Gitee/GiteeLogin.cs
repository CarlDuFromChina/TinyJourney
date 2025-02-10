using System;
using System.Collections.Generic;
using System.Text;
using Sixpence.Web.Model;
using Sixpence.Web.Service;
using Sixpence.Web.Entity;
using Sixpence.EntityFramework;
using Microsoft.Extensions.Logging;

namespace Sixpence.Web.Auth.Gitee
{
    public class GiteeLogin : IThirdPartyLoginStrategy
    {
        private readonly ILogger<GiteeLogin> _logger;
        private readonly GiteeAuthService _giteeAuthService;
        private readonly SysRoleService _sysRoleService;
        private readonly IEntityManager _manager;
        public GiteeLogin(ILogger<GiteeLogin> logger, GiteeAuthService giteeAuthService, SysRoleService sysRoleService, IEntityManager manager)
        {
            _logger = logger;
            _giteeAuthService = giteeAuthService;
            _sysRoleService = sysRoleService;
            _manager = manager;
        }

        public string GetName() => "Gitee";

        public LoginResponse Login(object param)
        {
            try
            {
                var code = param as string;
                var giteeToken = _giteeAuthService.GetAccessToken(code).Result;
                var giteeUser = _giteeAuthService.GetGiteeUserInfo(giteeToken).Result;
                var user = _manager.QueryFirst<SysUser>(new { gitee_id = giteeUser.id.ToString() });

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
                    var avatarId = _giteeAuthService.DownloadImage(giteeUser.avatar_url, id).Result;
                    var user = new SysUser()
                    {
                        Id = id,
                        Code = giteeUser.id.ToString(),
                        Password = null,
                        Name = giteeUser.name,
                        Mailbox = giteeUser.email,
                        Introduction = giteeUser.bio,
                        Avatar = avatarId,
                        RoleId = role.Id,
                        RoleName = role.Name,
                        GiteeId = giteeUser.id.ToString(),
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
                _logger.LogError("Gitee 登录失败：" + ex.Message, ex);
                return new LoginResponse() { result = false, message = "Gitee 登录失败" };
            }
            finally
            {
                _manager.Dispose();
            }
        }
    }
}
