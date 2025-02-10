using System;
using System.Collections.Generic;
using System.Text;
using Sixpence.Web.Service;
using Sixpence.Web.Entity;
using Sixpence.EntityFramework;

namespace Sixpence.Web.Auth.Gitee
{
    public class GiteeUserBind : IThirdPartyBindStrategy
    {
        private readonly IEntityManager _manager;
        private readonly GiteeAuthService _giteeAuthService;
        public GiteeUserBind(IEntityManager manager, GiteeAuthService giteeAuthService)
        {
            _manager = manager;
            _giteeAuthService = giteeAuthService;
        }
        public string GetName() => "Gitee";

        public void Bind(string code, string userid)
        {
            _manager.ExecuteTransaction(() =>
            {
                var user = _manager.QueryFirst<SysUser>(userid);
                var githubToken = _giteeAuthService.GetAccessToken(code, userid).Result;
                var githubUser = _giteeAuthService.GetGiteeUserInfo(githubToken).Result;
                user.GiteeId = githubUser.id.ToString();
                _manager.Update(user);
            });
        }
    }
}
