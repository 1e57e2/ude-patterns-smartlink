namespace Redirector;

public interface IRedirect
{
    Uri RedirectUrl { get; }
    IReadOnlyCollection<IRedirectRule> Rules { get; }
}