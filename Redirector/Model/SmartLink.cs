namespace Redirector;

public class SmartLink(IHttpContextAccessor context) : ISmartLink
{
    public string? Path => context.HttpContext?.Request.Path.Value;
}