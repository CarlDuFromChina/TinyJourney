using System;
using System.Collections.Generic;
using System.Text;
using Sixpence.Web.Service;
using Sixpence.Web.Model;
using Sixpence.Web.Entity;
using Sixpence.ORM;
using Microsoft.Extensions.Logging;

namespace Sixpence.Web.Auth.Github
{
    public class GithubLogin : IThirdPartyLoginStrategy
    {
        public string GetName() => "Github";

        public LoginResponse Login(object param)
        {
            var manager = new EntityManager();
            var githubService = new GithubAuthService(manager);
            var sysRoleService = new SysRoleService(manager);
            var logger = AppContext.GetLogger<GithubLogin>();

            try
            {
                var code = param as string;
                var githubToken = githubService.GetAccessToken(code);
                var githubUser = githubService.GetUserInfo(githubToken);
                var user = manager.QueryFirst<SysUser>(new { github_id = githubUser.id.ToString() });

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

                return manager.ExecuteTransaction(() =>
                {
                    var role = sysRoleService.GetGuest();
                    var id = Guid.NewGuid().ToString();
                    var avatarId = githubService.DownloadImage(githubUser.avatar_url, id);
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
                        isActiveName = "启用"
                    };
                    manager.Create(user, false);
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
                    manager.Create(_authUser);

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
                logger.LogError("Github 登录失败：" + ex.Message, ex);
                manager.Dispose();
                return new LoginResponse() { result = false, message = "Github 登录失败" };
            }
        }
    }
}
