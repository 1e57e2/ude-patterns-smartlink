using System.Text.Json;

namespace Redirector;

public interface IFileStorageRepositoryCache
{
    JsonElement? GetDescription(string? path);
}