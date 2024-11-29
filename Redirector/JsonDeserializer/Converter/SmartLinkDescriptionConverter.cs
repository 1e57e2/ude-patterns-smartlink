using System.Text.Json;
using System.Text.Json.Serialization;

namespace Redirector;

public class SmartLinkDescriptionConverter : JsonConverter<SmartLinkDescription>
{
    public override SmartLinkDescription Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);

        var element = document.RootElement;
        var linkPath = element.GetProperty("LinkPath").GetString();
        var description = element.Clone();

        return new SmartLinkDescription
        {
            LinkPath = linkPath!,
            Description = description
        };
    }

    public override void Write(Utf8JsonWriter writer, SmartLinkDescription value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        if (!value.Description.TryGetProperty("LinkPath", out _))
            writer.WriteString("LinkPath", value.LinkPath);
        foreach (var property in value.Description.EnumerateObject())
            property.WriteTo(writer);
        writer.WriteEndObject();
    }
}