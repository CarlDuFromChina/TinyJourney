using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Sixpence.Web.Model.Github
{
    public class GithubConfig
    {
        public string client_id { get; set; }

        [JsonIgnore]
        public string client_secret { get; set; }
    }
}
