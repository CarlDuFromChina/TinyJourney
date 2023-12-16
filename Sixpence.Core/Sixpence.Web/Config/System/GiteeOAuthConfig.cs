using System;

namespace Sixpence.Web.Config
{
    public class GiteeOAuthConfig : ISysConfig
    {
        public string Name => "Gitee OAuth";

        public string Code => "gitee_oauth";

        public object DefaultValue => "";

        public string Description => "Gitee OAuth";
    }
}

