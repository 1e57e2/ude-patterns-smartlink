using Microsoft.AspNetCore.Http;
using Moq;

namespace Redirector.Tests;

public class SmartLinkTests
{
    [Fact]
    public void Path_ShouldReturnRequestPath_WhenHttpContextAndRequestAreValid()
    {
        // Arrange
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var mockHttpContext = new Mock<HttpContext>();
        var mockHttpRequest = new Mock<HttpRequest>();

        var expectedPath = "/test-path";
        mockHttpRequest.Setup(r => r.Path).Returns(new PathString(expectedPath));
        mockHttpContext.Setup(c => c.Request).Returns(mockHttpRequest.Object);
        mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(mockHttpContext.Object);

        var smartLink = new SmartLink(mockHttpContextAccessor.Object);

        // Act
        var actualPath = smartLink.Path;

        // Assert
        Assert.Equal(expectedPath, actualPath);
    }

    [Fact]
    public void Path_ShouldReturnNull_WhenHttpContextIsNull()
    {
        // Arrange
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        mockHttpContextAccessor.Setup(a => a.HttpContext).Returns((HttpContext?)null);

        var smartLink = new SmartLink(mockHttpContextAccessor.Object);

        // Act
        var actualPath = smartLink.Path;

        // Assert
        Assert.Null(actualPath);
    }

    [Fact]
    public void Path_ShouldReturnNull_WhenRequestPathIsNull()
    {
        // Arrange
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var mockHttpContext = new Mock<HttpContext>();
        var mockHttpRequest = new Mock<HttpRequest>();

        mockHttpRequest.Setup(r => r.Path).Returns((PathString)null);
        mockHttpContext.Setup(c => c.Request).Returns(mockHttpRequest.Object);
        mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(mockHttpContext.Object);

        var smartLink = new SmartLink(mockHttpContextAccessor.Object);

        // Act
        var actualPath = smartLink.Path;

        // Assert
        Assert.Null(actualPath);
    }
}