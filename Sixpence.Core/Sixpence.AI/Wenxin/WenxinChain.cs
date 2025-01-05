namespace Sixpence.AI.Wenxin;

public class WenxinChain
{
    private WenxinClient _client;
    private PromptTemplate _promptTemplate;

    public WenxinChain(WenxinClient client, PromptTemplate promptTemplate)
    {
        _client = client;
        _promptTemplate = promptTemplate;
    }

    public async Task<string> RunAsync(Dictionary<string, string> variables)
    {
        string prompt = _promptTemplate.Format(variables);
        return await _client.CallApiAsync(prompt);
    }
}