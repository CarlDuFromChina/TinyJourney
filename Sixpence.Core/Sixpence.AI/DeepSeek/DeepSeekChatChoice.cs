using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.AI.DeepSeek
{
    /// <summary>
    /// 非流式响应中的单个回复单元
    /// </summary>
    public class DeepSeekChatChoice
    {
        public int Index { get; set; }

        /// <summary>
        /// 对应该 choice 的消息
        /// </summary>
        public DeepSeekChatMessage Message { get; set; }

        /// <summary>
        /// 回答结束的原因，比如 "stop", "max_tokens" 等
        /// </summary>
        public string FinishReason { get; set; }
    }

}
