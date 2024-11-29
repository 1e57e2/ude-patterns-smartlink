namespace Redirector;

public class RedirectableSmartLink : IRedirectableSmartLink
{
    public required IReadOnlyCollection<Redirect> Redirects { get; set; }
}