using System.Text.Json;

namespace Redirector;

public interface ISmartLinkDescrptionValidator
{
    bool TryGetDescription(JsonElement jsonElement, out SmartLinkDescription linkDescription, out string? message);
}