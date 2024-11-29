namespace Redirector;

public interface IRedirectsRepository
{
    Task<IReadOnlyCollection<IRedirect>> ReadAsync(CancellationToken cancellationToken = default);
}