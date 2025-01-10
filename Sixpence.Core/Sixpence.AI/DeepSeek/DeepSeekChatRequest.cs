using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.AI.DeepSeek
{
    /// <summary>
    /// 用于调用 DeepSeek 聊天接口的请求
    /// </summary>
    public class DeepSeekChatRequest
    {
        /// <summary>
        /// 消息列表：通常包含 system、user 等角色的对话内容
        /// </summary>
        public List<DeepSeekChatMessage> Messages { get; set; } = new();

        /// <summary>
        /// 一些可选的参数，如温度、top_p 等
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; } = new();
    }

}
