using Microsoft.AspNetCore.Http;
using Moq;
using BrowserValueProvider;

namespace Redirector.Tests;

public class BrowserValueProviderTests
{
    [Theory]
    [InlineData("Mozilla/5.0 (Windows NT 10.0; Win64; x64) Edg/91.0.864.59", "Microsoft Edge")]
    [InlineData("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36", "Chrome")]
    [InlineData("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:89.0) Gecko/20100101 Firefox/89.0", "Firefox")]
    [InlineData("Mozilla/5.0 (iPhone; CPU iPhone OS 14_6 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/14.1.1 Mobile/15E148 Safari/604.1", "Safari")]
    [InlineData("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36 OPR/77.0.4054.203", "Opera")]
    [InlineData("Mozilla/5.0 (Windows NT 10.0; Trident/7.0; rv:11.0) like Gecko", "Internet Explorer")]
    [InlineData("Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1)", "Internet Explorer")]
    [InlineData("Mozilla/5.0 (UnknownBrowser; Windows NT 10.0; Win64; x64)", "Unknown")]
    public void Value_ShouldReturnCorrectBrowserName(string userAgent, string expectedBrowser)
    {
        // Arrange
        var contextMock = new Mock<IHttpContextAccessor>();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["User-Agent"] = userAgent;
        contextMock.Setup(c => c.HttpContext).Returns(httpContext);
        var provider = new BrowserValueProvider.BrowserValueProvider(contextMock.Object);
        var getValue = (Func<string>)provider.ValueProvider;

        // Act
        var browser = getValue();

        // Assert
        Assert.Equal(expectedBrowser, browser);
    }

    [Fact]
    public void Value_ShouldReturnUnknown_WhenUserAgentIsEmpty()
    {
        // Arrange
        var contextMock = new Mock<IHttpContextAccessor>();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["User-Agent"] = string.Empty;
        contextMock.Setup(c => c.HttpContext).Returns(httpContext);
        var provider = new BrowserValueProvider.BrowserValueProvider(contextMock.Object);
        var getValue = (Func<string>)provider.ValueProvider;

        // Act
        var browser = getValue();

        // Assert
        Assert.Equal("Unknown", browser);
    }

    [Fact]
    public void Value_ShouldReturnUnknown_WhenHttpContextIsNull()
    {
        // Arrange
        var contextMock = new Mock<IHttpContextAccessor>();
        contextMock.Setup(c => c.HttpContext).Returns((HttpContext?)null);
        var provider = new BrowserValueProvider.BrowserValueProvider(contextMock.Object);
        var getValue = (Func<string>)provider.ValueProvider;

        // Act
        var browser = getValue();

        // Assert
        Assert.Equal("Unknown", browser);
    }
}
