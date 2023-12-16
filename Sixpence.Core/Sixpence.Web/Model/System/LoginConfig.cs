using Sixpence.Web.Model.Gitee;
using Sixpence.Web.Model.Github;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sixpence.Web.Model.System
{
    public class LoginConfig
    {
        public GithubConfig github { get; set; }
        public GiteeConfig gitee { get; set; }
    }
}
