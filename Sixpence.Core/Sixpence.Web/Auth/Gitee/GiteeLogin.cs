using System;
using System.Collections.Generic;
using System.Text;
using Sixpence.Web.Model;
using Sixpence.Web.Service;
using Sixpence.Web.Entity;
using Sixpence.ORM;
using Microsoft.Extensions.Logging;

namespace Sixpence.Web.Auth.Gitee
{
    public class GiteeLogin : IThirdPartyLoginStrategy
    {
        public string GetName() => "Gitee";

        public LoginResponse Login(object param)
        {
            var manager = new EntityManager();
            var giteeService = new GiteeAuthService(manager);
            var sysRoleService = new SysRoleService(manager);
            var logger = AppContext.GetLogger<GiteeLogin>();

            try
            {
                var code = param as string;
                var giteeToken = giteeService.GetAccessToken(code);
                var giteeUser = giteeService.GetGiteeUserInfo(giteeToken);
                var user = manager.QueryFirst<SysUser>(new { gitee_id = giteeUser.id.ToString() });
                
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
                    var avatarId = giteeService.DownloadImage(giteeUser.avatar_url, id);
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
                logger.LogError("Gitee 登录失败：" + ex.Message, ex);
                return new LoginResponse() { result = false, message = "Gitee 登录失败" };
            }
            finally
            {
                manager.Dispose();
            }
        }
    }
}
