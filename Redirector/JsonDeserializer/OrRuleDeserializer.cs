using System.Text.Json;

namespace Redirector;

public class OrRuleDeserializer : IRedirectRuleDeserializer
{
    public IRedirectRule Deserialize(JsonElement element, JsonSerializerOptions options)
    {
        var rules = element.GetProperty("Rules").Deserialize<List<IRedirectRule>>(options)
                    ?? throw new JsonException("Failed to deserialize 'Rules' for 'Or' operation.");

        return new RedirectRulesOr(rules);
    }
}