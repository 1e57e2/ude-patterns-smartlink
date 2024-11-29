using System.Text.Json;

namespace Redirector;

public class AndRuleDeserializer : IRedirectRuleDeserializer
{
    public IRedirectRule Deserialize(JsonElement element, JsonSerializerOptions options)
    {
        var rules = element.GetProperty("Rules").Deserialize<List<IRedirectRule>>(options)
                    ?? throw new JsonException("Failed to deserialize 'Rules' for 'And' operation.");

        return new RedirectRulesAnd(rules);
    }
}