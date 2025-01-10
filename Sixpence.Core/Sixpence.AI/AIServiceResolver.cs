using Sixpence.AI.DeepSeek;
using Sixpence.AI.Wenxin;
using Sixpence.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.AI
{
    public class AIServiceResolver
    {
        public static bool Resolve(object item)
        {
            var aiPlatform = AIPlatformConfig.Config.DefaultAI.ToEnum<AIPlatformEnum>();
            return aiPlatform switch
            {
                AIPlatformEnum.BaiduWenxin => item is WenxinAIService,
                AIPlatformEnum.DeepSeek => item is DeepSeekAIService,
                _ => false
            };
        }
    }
}
