namespace Redirector;

public class RedirectMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext, IRedirectService redirectService)
    {
        if (!httpContext.Request.HttpContext.Request.Path.StartsWithSegments("/api"))
        {
            var cancellationToken = httpContext.RequestAborted;
            var redirect = await redirectService.EvaluateAsync(cancellationToken);
            if (redirect is not null)
            {
                httpContext.Response.StatusCode = StatusCodes.Status307TemporaryRedirect;
                httpContext.Response.Headers.Append("Location", redirect.AbsoluteUri);
                return;
            }
        }

        await next(httpContext);
    }
}