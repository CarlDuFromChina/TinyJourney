using Sixpence.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.AI.Wenxin
{
    public class WenxinAIService : IAIService
    {
        public async Task<string> ProcessChatAsync(string message)
        {
            var config = AIPlatformConfig.Config.BaiduWenxin;

            // 初始化文心客户端
            var wenxinClient = new WenxinClient(config.ApiKey, config.SecretKey);

            // 获取 Access Token
            if (!await wenxinClient.AuthenticateAsync())
            {
                throw new SpException("文心 API 认证失败，请检查 API Key 和 Secret Key。");
            }

            try
            {
                string result = await wenxinClient.CallApiAsync(message);
                return result;
            }
            catch (Exception ex)
            {
                throw new SpException($"发生错误: {ex.Message}");
            }
        }

        public async Task<string> ProcessChatTemplateAsync(PromptTemplate template, Dictionary<string, string> variables)
        {
            var config = AIPlatformConfig.Config.BaiduWenxin;
            
            // 初始化文心客户端
            var wenxinClient = new WenxinClient(config.ApiKey, config.SecretKey);

            // 获取 Access Token
            if (!await wenxinClient.AuthenticateAsync())
            {
                throw new SpException("文心 API 认证失败，请检查 API Key 和 Secret Key。");
            }

            // 创建 WenxinChain
            WenxinChain chain = new WenxinChain(wenxinClient, template);

            // 执行链式调用
            try
            {
                string result = await chain.RunAsync(variables);
                return result;
            }
            catch (Exception ex)
            {
                throw new SpException($"发生错误: {ex.Message}");
            }
        }
    }
}
