using System;
using System.Collections.Generic;
using System.Text;
using Sixpence.Web.Service;
using Sixpence.Web.Entity;
using Sixpence.ORM;

namespace Sixpence.Web.Auth.Github
{
    public class GithubUserBind : IThirdPartyBindStrategy
    {
        public string GetName() => "Github";

        public void Bind(string code, string userid)
        {
            using (var manager = new EntityManager())
            {
                var githubService = new GithubAuthService(manager);
                manager.ExecuteTransaction(() =>
                {
                    var user = manager.QueryFirst<SysUser>(userid);
                    var githubToken = githubService.GetAccessToken(code).Result;
                    var githubUser = githubService.GetUserInfo(githubToken).Result;
                    user.GithubId = githubUser.id.ToString();
                    manager.Update(user);
                });
            }
        }
    }
}
