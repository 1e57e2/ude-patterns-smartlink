using Moq;

namespace Redirector.Tests;

public class RedirectServiceTests
{
    [Fact]
    public async Task ShouldReturnFirstValidRedirectUrl()
    {
        // Arrange
        var redirectsRepositoryMock = new Mock<IRedirectsRepository>();
        redirectsRepositoryMock.Setup(r => r.ReadAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<IRedirect>
            {
                new Redirect
                {
                    RedirectUrl = new Uri("https://example.org"),
                    Rules = new List<IRedirectRule> { Mock.Of<IRedirectRule>(r => r.Evaluate() == true) }
                },
                new Redirect
                {
                    RedirectUrl = new Uri("https://example.com"),
                    Rules = new List<IRedirectRule> { Mock.Of<IRedirectRule>(r => r.Evaluate() == false) }
                }
            });

        var service = new RedirectService(redirectsRepositoryMock.Object);

        // Act
        var result = await service.EvaluateAsync(CancellationToken.None);

        // Assert
        Assert.Equal(new Uri("https://example.org"), result);
    }

    [Fact]
    public async Task ShouldReturnNull_WhenNoValidRedirectFound()
    {
        // Arrange
        var redirectsRepositoryMock = new Mock<IRedirectsRepository>();
        redirectsRepositoryMock.Setup(r => r.ReadAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<IRedirect>());

        var service = new RedirectService(redirectsRepositoryMock.Object);

        // Act
        var result = await service.EvaluateAsync(CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
