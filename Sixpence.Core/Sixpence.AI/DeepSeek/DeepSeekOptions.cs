namespace Sixpence.AI.DeepSeek;

public class DeepSeekOptions
{
    /// <summary>
    /// DeepSeek API 的基础 URL，格式如：https://your-deepseek-endpoint.com
    /// </summary>
    public string BaseUrl { get; set; }

    /// <summary>
    /// 认证所需的 API Key 或者 Token
    /// </summary>
    public string ApiKey { get; set; }

    /// <summary>
    /// 超时设置，默认 30 秒
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// 是否在遇到非 200 响应时抛出异常
    /// </summary>
    public bool ThrowOnErrorStatusCode { get; set; } = true;
}
