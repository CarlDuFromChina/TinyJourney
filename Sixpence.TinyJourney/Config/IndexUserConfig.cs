using System;
using Sixpence.Web;

namespace Sixpence.TinyJourney.Config
{
    public class IndexUserConfig : ISysConfig
    {
        public string Name => "主页用户";

        public string Code => "index_user";

        public object DefaultValue => "";

        public string Description => "值内容填入用户编码，则主页则显示该用户的信息";
    }
}
