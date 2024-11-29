namespace Redirector;

public class NotFoundMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext, IStatableSmartLinkRepository statableSmartLinkRepository)
    {
        if (!httpContext.Request.HttpContext.Request.Path.StartsWithSegments("/api"))
        {
            var cancellationToken = httpContext.RequestAborted;
            var statableSmartLink = await statableSmartLinkRepository.ReadAsync(cancellationToken);
            if (statableSmartLink is null || statableSmartLink.State == "deleted")
            {
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }
        }
        await next(httpContext);
    }
}