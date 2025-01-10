using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.AI.DeepSeek
{
    /// <summary>
    /// 表示对话中的单条消息
    /// </summary>
    public class DeepSeekChatMessage
    {
        /// <summary>
        /// 消息的角色，如 "system" / "user" / "assistant"
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// 消息的具体内容
        /// </summary>
        public string Content { get; set; }
    }

}
