using Microsoft.AspNetCore.Http;
using Redirector;

namespace BrowserValueProvider;

[RedirectSubjectName("Browser")]
public class BrowserValueProvider(IHttpContextAccessor context) : SubjectValueProvider<string?>
{
    protected override string? Value
    {
        get
        {
            var userAgent = context.HttpContext?.Request.Headers.UserAgent.ToString() ?? string.Empty;
            if (userAgent.Contains("Edg"))
                return "Microsoft Edge";
            if (userAgent.Contains("Opera") || userAgent.Contains("OPR"))
                return "Opera";
            if (userAgent.Contains("Firefox"))
                return "Firefox";
            if (userAgent.Contains("Chrome"))
                return "Chrome";
            if (userAgent.Contains("Safari"))
                return "Safari";
            if (userAgent.Contains("Trident") || userAgent.Contains("MSIE"))
                return "Internet Explorer";
            return "Unknown";
        }
    }
}