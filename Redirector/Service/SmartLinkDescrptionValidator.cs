using System.Text.Json;

namespace Redirector;

public class SmartLinkDescrptionValidator(JsonSerializerOptions serializerOptions) : ISmartLinkDescrptionValidator
{
    public bool TryGetDescription(JsonElement jsonElement, out SmartLinkDescription linkDescription, out string? message)
    {
        linkDescription = null!;
        message = null;
        try
        {
            linkDescription = jsonElement.Deserialize<SmartLinkDescription>(serializerOptions)!;
            _ = linkDescription.Description.Deserialize<RedirectableSmartLink>(serializerOptions);
            _ = linkDescription.Description.Deserialize<StatableSmartLink>(serializerOptions);
        }
        catch (JsonException e)
        {
            message = e.InnerException is not null ? e.InnerException.Message : e.Message;
            return false;
        }
        return true;
    }
}