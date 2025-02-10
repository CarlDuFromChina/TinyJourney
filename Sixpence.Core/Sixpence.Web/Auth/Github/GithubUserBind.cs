using System;
using System.Collections.Generic;
using System.Text;
using Sixpence.Web.Service;
using Sixpence.Web.Entity;
using Sixpence.EntityFramework;

namespace Sixpence.Web.Auth.Github
{
    public class GithubUserBind : IThirdPartyBindStrategy
    {
        private readonly IEntityManager _manager;
        private readonly GithubAuthService _githubAuthService;
        public GithubUserBind(IEntityManager manager, GithubAuthService githubAuthService)
        {
            _manager = manager;
            _githubAuthService = githubAuthService;
        }

        public string GetName() => "Github";

        public void Bind(string code, string userid)
        {
            _manager.ExecuteTransaction(() =>
            {
                var user = _manager.QueryFirst<SysUser>(userid);
                var githubToken = _githubAuthService.GetAccessToken(code).Result;
                var githubUser = _githubAuthService.GetUserInfo(githubToken).Result;
                user.GithubId = githubUser.id.ToString();
                _manager.Update(user);
            });
        }
    }
}
