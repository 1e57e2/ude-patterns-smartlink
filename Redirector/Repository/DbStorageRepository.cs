using System.Text.Json;

namespace Redirector;

public class DbStorageRepository(RedirectsDbContext context, ISmartLink smartLink) : IStorageRepository
{
    public async Task<JsonElement?> GetDescriptionAsync(CancellationToken cancellationToken)
    {
        var result = await context.SmartLinkDescription.FindAsync(smartLink.Path, cancellationToken);
        return result?.Description;
    }
}