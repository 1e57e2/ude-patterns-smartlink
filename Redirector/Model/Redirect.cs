namespace Redirector;

public class Redirect : IRedirect
{
    public required Uri RedirectUrl { get; init; }

    public required IReadOnlyCollection<IRedirectRule> Rules { get; init; }
}