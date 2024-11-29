namespace Redirector;

public class RedirectService(IRedirectsRepository redirectsRepository) : IRedirectService
{
    public async Task<Uri?> EvaluateAsync(CancellationToken cancellationToken)
    {
        var redirects = await redirectsRepository.ReadAsync(cancellationToken);
        return redirects
            .Where(redirect => redirect.Rules.All(r => r.Evaluate()))
            .Select(redirect => redirect.RedirectUrl)
            .FirstOrDefault();
    }
}