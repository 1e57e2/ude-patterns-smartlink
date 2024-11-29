using System.Text.Json;

namespace Redirector;

public interface IStorageRepository
{
    Task<JsonElement?> GetDescriptionAsync(CancellationToken cancellationToken = default);
}