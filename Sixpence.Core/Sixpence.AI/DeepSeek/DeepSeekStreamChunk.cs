using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.AI.DeepSeek
{
    /// <summary>
    /// 流式返回的增量数据
    /// </summary>
    public class DeepSeekStreamChunk
    {
        /// <summary>
        /// 对话 ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 用于区分事件类型，如 delta / complete
        /// </summary>
        public string Event { get; set; }

        /// <summary>
        /// 包含具体内容
        /// </summary>
        public List<DeepSeekStreamChoice> Choices { get; set; } = new();
    }

    /// <summary>
    /// 每个 choice 的增量数据
    /// </summary>
    public class DeepSeekStreamChoice
    {
        public DeepSeekChatDelta Delta { get; set; }
        public string FinishReason { get; set; }
    }

    /// <summary>
    /// 对话内容的增量，如一个字/一句话
    /// </summary>
    public class DeepSeekChatDelta
    {
        public string Role { get; set; }
        public string Content { get; set; }
    }

}
