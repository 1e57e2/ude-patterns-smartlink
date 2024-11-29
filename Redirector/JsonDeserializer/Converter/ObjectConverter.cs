using System.Text.Json;
using System.Text.Json.Serialization;

namespace Redirector;

public class ObjectConverter : JsonConverter<object>
{
    public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions? options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.String => reader.GetString(),
            JsonTokenType.Number when reader.TryGetInt32(out var intValue) => intValue,
            JsonTokenType.Number when reader.TryGetInt64(out var intValue) => intValue,
            JsonTokenType.Number => reader.GetDouble(),
            JsonTokenType.False => false,
            JsonTokenType.True => true,
            JsonTokenType.StartArray => JsonSerializer.Deserialize<List<object>>(ref reader, options),
            JsonTokenType.StartObject => JsonSerializer.Deserialize<Dictionary<string, object>>(ref reader, options),
            JsonTokenType.Null => null,
            JsonTokenType.None => null,
            // EndArray, EndObject, Comment, PropertyName
            _ => throw new InvalidOperationException($"Unsupported JSON token type: {reader.TokenType}")
        };
    }

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}