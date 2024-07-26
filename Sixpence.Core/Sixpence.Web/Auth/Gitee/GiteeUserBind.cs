using System;
using System.Collections.Generic;
using System.Text;
using Sixpence.Web.Service;
using Sixpence.Web.Entity;
using Sixpence.ORM;

namespace Sixpence.Web.Auth.Gitee
{
    public  class GiteeUserBind : IThirdPartyBindStrategy
    {
        public string GetName() => "Gitee";

        public void Bind(string code, string userid)
        {
            using (var manager = new EntityManager())
            {
                var giteeAuthService = new GiteeAuthService(manager);
                manager.ExecuteTransaction(() =>
                {
                    var user = manager.QueryFirst<SysUser>(userid);
                    var githubToken = giteeAuthService.GetAccessToken(code, userid).Result;
                    var githubUser = giteeAuthService.GetGiteeUserInfo(githubToken).Result;
                    user.GiteeId = githubUser.id.ToString();
                    manager.Update(user);
                });
            }
        }
    }
}
