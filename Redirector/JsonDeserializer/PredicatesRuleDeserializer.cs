using System.Text.Json;

namespace Redirector;

public class PredicatesRuleDeserializer(IPredicateFactory predicateFactory) : IRedirectRuleDeserializer
{
    public IRedirectRule Deserialize(JsonElement element, JsonSerializerOptions options)
    {
        var subjectName = element.GetProperty("SubjectName").GetString()
                          ?? throw new JsonException("Missing 'SubjectName' property.");

        var predicates = element.GetProperty("Predicates").Deserialize<Dictionary<string, object>>(options)
                         ?? throw new JsonException($"Failed to deserialize 'Predicates' for subject '{subjectName}'.");

        var predicateRules = predicates.Select(p =>
            predicateFactory.CreatePredicate(subjectName, p.Key, p.Value)).ToList();

        return new RedirectRulesAnd(predicateRules);
    }
}