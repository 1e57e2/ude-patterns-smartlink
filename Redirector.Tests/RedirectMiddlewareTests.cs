using Microsoft.AspNetCore.Http;
using Moq;

namespace Redirector.Tests;

public class RedirectMiddlewareTests
{
    [Fact]
    public async Task ShouldReturn307WithLocationHeader_WhenRedirectIsFound()
    {
        // Arrange
        var middleware = new RedirectMiddleware(context => Task.CompletedTask);
        var httpContext = new DefaultHttpContext();
        var redirectServiceMock = new Mock<IRedirectService>();
        redirectServiceMock.Setup(s => s.EvaluateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Uri("https://example.org/"));

        // Act
        await middleware.InvokeAsync(httpContext, redirectServiceMock.Object);

        // Assert
        Assert.Equal(StatusCodes.Status307TemporaryRedirect, httpContext.Response.StatusCode);
        Assert.Equal("https://example.org/", httpContext.Response.Headers["Location"]);
    }

    [Fact]
    public async Task ShouldCallNext_WhenNoRedirectIsFound()
    {
        // Arrange
        var nextCalled = false;
        var middleware = new RedirectMiddleware(context =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        var httpContext = new DefaultHttpContext();
        var redirectServiceMock = new Mock<IRedirectService>();
        redirectServiceMock.Setup(s => s.EvaluateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((Uri?)null);

        // Act
        await middleware.InvokeAsync(httpContext, redirectServiceMock.Object);

        // Assert
        Assert.True(nextCalled);
    }
}
