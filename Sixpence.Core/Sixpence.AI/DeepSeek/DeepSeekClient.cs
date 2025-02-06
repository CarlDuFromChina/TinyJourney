using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sixpence.Common;

namespace Sixpence.AI.DeepSeek;

public class DeepSeekClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly DeepSeekOptions _options;
    private bool _disposed;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="options">DeepSeek 连接配置</param>
    public DeepSeekClient(DeepSeekOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_options.BaseUrl),
            Timeout = _options.Timeout
        };

        // 如果使用 Token 认证，可在 Header 中添加 Authorization
        if (!string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _options.ApiKey);
        }
    }

    /// <summary>
    /// 发起一次非流式的聊天请求
    /// </summary>
    /// <param name="request">聊天请求，包含对话消息数组</param>
    /// <returns>返回完整的对话响应</returns>
    public async Task<DeepSeekChatResponse> ChatAsync(DeepSeekChatRequest request)
    {
        string url = "/chat/completions";  // 假设的非流式接口
        using var content = BuildJsonContent(request);
        using var response = await _httpClient.PostAsync(url, content);

        await EnsureSuccessStatusCodeAsync(response);

        // 反序列化完整对话响应
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<DeepSeekChatResponse>(responseString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    /// <summary>
    /// 发起一次流式聊天请求（SSE 示例）
    /// 返回一个可异步迭代的流，逐段返回数据
    /// </summary>
    /// <param name="request">聊天请求，包含对话消息数组</param>
    /// <returns>IAsyncEnumerable，遍历时可不断获取返回的增量数据</returns>
    public async IAsyncEnumerable<DeepSeekStreamChunk> ChatStreamAsync(DeepSeekChatRequest request)
    {
        string url = "/api/chat/stream"; // 假设的流式接口
        using var content = BuildJsonContent(request);
        using var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, url) { Content = content }, HttpCompletionOption.ResponseHeadersRead);

        await EnsureSuccessStatusCodeAsync(response);

        // 以流的方式读取内容
        using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();

            // SSE 一般会返回形如 "data: {...}"，需要进行解析
            if (string.IsNullOrWhiteSpace(line))
                continue;

            // 尝试从每一行中解析 SSE
            if (line.StartsWith("data: "))
            {
                string jsonPart = line.Substring("data: ".Length).Trim();

                // SSE 结束标志
                if (jsonPart == "[DONE]" || jsonPart == "DONE")
                {
                    yield break;
                }

                // 解析 JSON 片段
                var chunk = ParseSseLine(jsonPart);
                if (chunk != null)
                {
                    yield return chunk;
                }
            }
        }
    }

    /// <summary>
    /// 解析 SSE 返回的一行 JSON，并转换为 DeepSeekStreamChunk
    /// </summary>
    private DeepSeekStreamChunk ParseSseLine(string jsonPart)
    {
        try
        {
            return JsonSerializer.Deserialize<DeepSeekStreamChunk>(jsonPart, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch
        {
            // 如果解析失败，可根据需要处理（忽略或抛异常）
            return null;
        }
    }

    /// <summary>
    /// 处理响应状态码
    /// </summary>
    /// <param name="response">HttpResponseMessage</param>
    private async Task EnsureSuccessStatusCodeAsync(HttpResponseMessage response)
    {
        if (_options.ThrowOnErrorStatusCode && !response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"请求失败，状态码：{(int)response.StatusCode}, 响应内容：{errorBody}");
        }
    }

    /// <summary>
    /// 序列化 JSON 内容
    /// </summary>
    /// <param name="obj">要序列化的对象</param>
    /// <returns>StringContent</returns>
    private StringContent BuildJsonContent(object obj)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = new SnakeCaseNamingPolicy(),
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
        var json = JsonSerializer.Serialize(obj, options);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    #region IDisposable

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _httpClient?.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
