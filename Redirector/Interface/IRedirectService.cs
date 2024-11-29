namespace Redirector;

public interface IRedirectService
{
    public Task<Uri?> EvaluateAsync(CancellationToken cancellationToken = default);
}