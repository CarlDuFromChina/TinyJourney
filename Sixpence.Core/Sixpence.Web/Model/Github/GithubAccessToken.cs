using System;
using System.Collections.Generic;
using System.Text;

namespace Sixpence.Web.Model.Github
{
    public class GithubAccessToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string scope { get; set; }
    }
}
