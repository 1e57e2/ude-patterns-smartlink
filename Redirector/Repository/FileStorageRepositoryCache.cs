using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Redirector;

public class FileStorageRepositoryCache : IFileStorageRepositoryCache
{
    private readonly Dictionary<string, JsonElement> _linkRedirects = new();

    public FileStorageRepositoryCache(ILogger<FileStorageRepositoryCache> logger,
        IOptions<FileStorageRepositoryOptions> options)
    {
        var filePath = options.Value.FilePath;
        if (filePath is null)
            return;
        logger.LogDebug($"Reading all rules from {filePath}");
        var json = File.ReadAllText(filePath);

        var doc = JsonDocument.Parse(json);
        foreach (var element in doc.RootElement.EnumerateArray())
        {
            var linkPath = element.GetProperty("LinkPath").GetString()
                           ?? throw new JsonException("LinkPath cannot be null");
            _linkRedirects[linkPath] = element;
        }
    }

    public JsonElement? GetDescription(string? linkPath)
    {
        return linkPath is not null && _linkRedirects.TryGetValue(linkPath, out var ret) ? ret : null;
    }
}