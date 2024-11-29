using Microsoft.AspNetCore.Http;
using Moq;

namespace Redirector.Tests;

public class NotFoundMiddlewareTests
{
    [Fact]
    public async Task ShouldReturn404_WhenLinkStateIsDeleted()
    {
        // Arrange
        var middleware = new NotFoundMiddleware(context => Task.CompletedTask);
        var httpContext = new DefaultHttpContext();
        var repositoryMock = new Mock<IStatableSmartLinkRepository>();
        repositoryMock.Setup(r => r.ReadAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StatableSmartLink { State = "deleted" });

        // Act
        await middleware.InvokeAsync(httpContext, repositoryMock.Object);

        // Assert
        Assert.Equal(StatusCodes.Status404NotFound, httpContext.Response.StatusCode);
    }

    [Fact]
    public async Task ShouldCallNext_WhenLinkIsValid()
    {
        // Arrange
        var nextCalled = false;
        var middleware = new NotFoundMiddleware(context =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        var httpContext = new DefaultHttpContext();
        var repositoryMock = new Mock<IStatableSmartLinkRepository>();
        repositoryMock.Setup(r => r.ReadAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StatableSmartLink { State = "enabled" });

        // Act
        await middleware.InvokeAsync(httpContext, repositoryMock.Object);

        // Assert
        Assert.True(nextCalled);
    }
}
