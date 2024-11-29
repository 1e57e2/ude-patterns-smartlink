namespace Redirector;

public interface IStatableSmartLinkRepository
{
    Task<IStatableSmartLink?> ReadAsync(CancellationToken cancellationToken = default);
}