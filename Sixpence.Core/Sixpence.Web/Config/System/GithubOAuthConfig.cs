using System;
namespace Sixpence.Web.Config
{
    public class GithubOAuthConfig : ISysConfig
    {
        public string Name => "Github OAuth";

        public string Code => "github_oauth";

        public object DefaultValue => "";

        public string Description => "Github OAuth";
    }
}

