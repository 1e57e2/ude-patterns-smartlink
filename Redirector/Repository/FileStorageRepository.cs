using System.Text.Json;

namespace Redirector;

public class FileStorageRepository(IFileStorageRepositoryCache cache, ISmartLink smartLink) : IStorageRepository
{
    public Task<JsonElement?> GetDescriptionAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(cache.GetDescription(smartLink.Path));
    }
}