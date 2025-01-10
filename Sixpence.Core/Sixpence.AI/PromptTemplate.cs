namespace Sixpence.AI;

public class PromptTemplate
{
    private string _template;

    public PromptTemplate(string template)
    {
        _template = template;
    }

    public string Format(Dictionary<string, string> variables)
    {
        string result = _template;
        foreach (var kv in variables)
        {
            result = result.Replace($"{{{kv.Key}}}", kv.Value);
        }
        return result;
    }
}