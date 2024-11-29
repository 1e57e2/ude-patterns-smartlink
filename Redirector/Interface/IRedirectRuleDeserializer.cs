using System.Text.Json;

namespace Redirector;

public interface IRedirectRuleDeserializer
{
    IRedirectRule Deserialize(JsonElement element, JsonSerializerOptions options);
}