using Sixpence.Common.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sixpence.Web.Config
{
    public class MailConfig : BaseAppConfig<MailConfig>
    {
        public string Name { get; set; }
        public string SMTP { get; set; }
        public string Password { get; set; }
    }
}
