namespace Redirector;

public interface IRedirectableSmartLink
{
    IReadOnlyCollection<Redirect> Redirects { get; set; }
}