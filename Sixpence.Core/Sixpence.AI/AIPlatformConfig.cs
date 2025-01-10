using Sixpence.Common.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.AI
{
    public class AIPlatformConfig : BaseAppConfig<AIPlatformConfig>
    {
        public string DefaultAI { get; set; }

        public WenxinApiConfig BaiduWenxin { get; set; }

        public DeepSeekConfig DeepSeek { get; set; }
    }

    public class WenxinApiConfig
    {
        public string ApiKey { get; set; }
        public string SecretKey { get; set; }
    }

    public class DeepSeekConfig
    {
        public string ApiKey { get; set; }
    }

    public enum AIPlatformEnum
    {
        BaiduWenxin = 1,
        DeepSeek = 2
    }
}
