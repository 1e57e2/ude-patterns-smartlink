using System.Collections.ObjectModel;
using System.Text.Json;

namespace Redirector;

public class RedirectsRepository(
    IStorageRepository storageRepository,
    JsonSerializerOptions serializerOptions
) : IRedirectsRepository
{
    public async Task<IReadOnlyCollection<IRedirect>> ReadAsync(CancellationToken cancellationToken)
    {
        var redirectsDescription = await storageRepository.GetDescriptionAsync(cancellationToken);
        
        if (!redirectsDescription.HasValue)
            return ReadOnlyCollection<Redirect>.Empty;

        var redirectableSmartLink = redirectsDescription.Value.Deserialize<RedirectableSmartLink>(serializerOptions);
        return redirectableSmartLink is null ? ReadOnlyCollection<Redirect>.Empty : redirectableSmartLink.Redirects;
    }
}