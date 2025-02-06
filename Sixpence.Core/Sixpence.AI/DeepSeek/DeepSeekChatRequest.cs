using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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
        /// 使用的模型的 ID。您可以使用 deepseek-chat, deepseek-reasoner
        /// </summary>
        public string Model { get; set; } = "deepseek-chat";

        /// <summary>
        /// 如果设置为 True，将会以 SSE（server-sent events）的形式以流式发送消息增量。消息流以 data: [DONE] 结尾。
        /// </summary>
        public bool Stream { get; set; } = false;
    }

}
