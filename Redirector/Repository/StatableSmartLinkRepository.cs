using System.Text.Json;

namespace Redirector;

public class StatableSmartLinkRepository(IStorageRepository storageRepository) : IStatableSmartLinkRepository
{
    public async Task<IStatableSmartLink?> ReadAsync(CancellationToken cancellationToken)
    {
        var smartLink = await storageRepository.GetDescriptionAsync(cancellationToken);
        return smartLink?.Deserialize<StatableSmartLink>();
    }
}