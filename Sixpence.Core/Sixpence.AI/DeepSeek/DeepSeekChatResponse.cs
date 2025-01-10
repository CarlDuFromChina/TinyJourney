using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.AI.DeepSeek
{
    /// <summary>
    /// 非流式对话响应模型
    /// </summary>
    public class DeepSeekChatResponse
    {
        /// <summary>
        /// 对话 ID 或者会话 ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 对象类型，可能是 "chat.completion"
        /// </summary>
        public string Object { get; set; }

        /// <summary>
        /// 生成时间戳
        /// </summary>
        public long Created { get; set; }

        /// <summary>
        /// 模型名称
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// 用量统计
        /// </summary>
        public DeepSeekChatUsage Usage { get; set; }

        /// <summary>
        /// 回复的 choices 数组
        /// </summary>
        public List<DeepSeekChatChoice> Choices { get; set; } = new();
    }

}
