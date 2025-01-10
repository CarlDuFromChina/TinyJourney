using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.AI
{
    public interface IAIService
    {
        /// <summary>
        /// 处理对话模板
        /// </summary>
        /// <param name="template">模板</param>
        /// <param name="variables">变量</param>
        /// <returns></returns>
        public Task<string> ProcessChatTemplateAsync(PromptTemplate template, Dictionary<string, string> variables);

        /// <summary>
        /// 处理对话
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task<string> ProcessChatAsync(string message);
    }
}
