using System.Text.Json;
using System.Text.Json.Serialization;

namespace Redirector;

public class RedirectRuleConverter(IKeyedServiceProviderWrapper serviceProviderWrapper) : JsonConverter<IRedirectRule>
{
    public override IRedirectRule Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected start of object.");

        using var document = JsonDocument.ParseValue(ref reader);
        var rootElement = document.RootElement;

        var operation = GetOperation(rootElement);

        var deserializer = serviceProviderWrapper.GetRequiredKeyedService<IRedirectRuleDeserializer>(operation);
        return deserializer.Deserialize(rootElement, options);
    }

    public override void Write(Utf8JsonWriter writer, IRedirectRule value, JsonSerializerOptions options)
    {
        throw new NotImplementedException("Serialization is not supported.");
    }

    private static string GetOperation(JsonElement element)
    {
        const string defaultOperation = "Predicates";
        element.TryGetProperty("Operation", out var operationProperty);
        return operationProperty.ValueKind != JsonValueKind.Undefined
            ? operationProperty.GetString() ?? defaultOperation
            : defaultOperation;
    }
}