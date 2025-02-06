using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.AI.DeepSeek
{
    public class DeepSeekAIService : IAIService
    {
        public async Task<string> ProcessChatAsync(string message)
        {
            // 1. 初始化配置
            var options = new DeepSeekOptions
            {
                BaseUrl = "https://api.deepseek.com",
                ApiKey = AIPlatformConfig.Config.DeepSeek.ApiKey,
                Timeout = TimeSpan.FromSeconds(60)
            };

            // 2. 初始化 DeepSeekClient
            using var client = new DeepSeekClient(options);

            // 3. 发起一次非流式对话
            var chatRequest = new DeepSeekChatRequest
            {
                Messages = new List<DeepSeekChatMessage>
                {
                    new DeepSeekChatMessage { Role = "user",   Content = message }
                },
            };

            var chatResponse = await client.ChatAsync(chatRequest);

            if (chatResponse?.Choices?.Count > 0)
            {
                var firstChoice = chatResponse.Choices[0];
                return firstChoice.Message.Content;
            }

            throw new Exception("对话失败：" + JsonConvert.SerializeObject(chatResponse));
        }

        public async Task<string> ProcessChatTemplateAsync(PromptTemplate template, Dictionary<string, string> variables)
        {
            // 1. 初始化配置
            var options = new DeepSeekOptions
            {
                BaseUrl = "https://api.deepseek.com",
                ApiKey = AIPlatformConfig.Config.DeepSeek.ApiKey,
                Timeout = TimeSpan.FromSeconds(120)
            };

            // 2. 初始化 DeepSeekClient
            using var client = new DeepSeekClient(options);

            // 3. 发起一次非流式对话
            var chatRequest = new DeepSeekChatRequest
            {
                Messages = new List<DeepSeekChatMessage>
                {
                    new DeepSeekChatMessage { Role = "user",   Content = template.Format(variables) }
                },
                Stream = false
            };

            var chatResponse = await client.ChatAsync(chatRequest);

            if (chatResponse?.Choices?.Count > 0)
            {
                var firstChoice = chatResponse.Choices[0];
                return firstChoice.Message.Content;
            }
            
            throw new Exception("对话失败：" + JsonConvert.SerializeObject(chatResponse));
        }
    }
}
