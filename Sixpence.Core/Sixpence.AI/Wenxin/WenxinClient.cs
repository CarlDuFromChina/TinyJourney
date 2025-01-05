using System.Text;
using Newtonsoft.Json;

namespace Sixpence.AI.Wenxin;

public class WenxinClient
{
    private string _apiKey;
    private string _secretKey;
    private string _accessToken;

    public WenxinClient(string apiKey, string secretKey)
    {
        _apiKey = apiKey;
        _secretKey = secretKey;
    }

    // 获取 Access Token
    public async Task<bool> AuthenticateAsync()
    {
        using (HttpClient client = new HttpClient())
        {
            string url = $"https://aip.baidubce.com/oauth/2.0/token?grant_type=client_credentials&client_id={_apiKey}&client_secret={_secretKey}";
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                dynamic json = JsonConvert.DeserializeObject(result);
                _accessToken = json.access_token;
                return true;
            }
        }
        return false;
    }

    // 调用文心 API
    public async Task<string> CallApiAsync(string prompt)
    {
        if (string.IsNullOrEmpty(_accessToken))
            throw new InvalidOperationException("Access token is not initialized. Please authenticate first.");

        using (HttpClient client = new HttpClient())
        {
            string url = $"https://aip.baidubce.com/rpc/2.0/ai_custom/v1/wenxinworkshop/chat/completions_pro?access_token={_accessToken}";
            var requestData = new
            {
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                temperature = 0.8,
                top_p = 0.8,
                penalty_score = 1,
                disable_search = false,
                enable_citation = false,
                collapsed = true
            };
            string jsonData = JsonConvert.SerializeObject(requestData);
            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                dynamic json = JsonConvert.DeserializeObject(result);
                return json.result; // 返回文心模型的回复
            }
            else
            {
                string error = await response.Content.ReadAsStringAsync();
                throw new Exception($"文心 API 调用失败: {error}");
            }
        }
    }
}