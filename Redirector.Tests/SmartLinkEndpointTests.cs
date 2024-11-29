using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Redirector.Tests;

public class EndpointsTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetAllSmartLinks_ReturnsOkWithList()
    {
        var response = await _client.GetAsync("/api/smartlinks");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var links = JsonSerializer.Deserialize<List<JsonElement>>(json);

        Assert.NotNull(links);
        Assert.NotEmpty(links);
        Assert.Equal("/test", links[0].GetProperty("LinkPath").GetString());
    }

    [Fact]
    public async Task GetSmartLinkByPath_ReturnsOkWithSmartLink()
    {
        var path = Uri.EscapeDataString("/test");
        var response = await _client.GetAsync($"/api/smartlinks/{path}");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var link = JsonSerializer.Deserialize<JsonElement>(json);

        Assert.Equal("/test", link.GetProperty("LinkPath").GetString());
        Assert.Equal("enabled", link.GetProperty("State").GetString());
    }

    [Fact]
    public async Task CreateSmartLink_ReturnsCreated()
    {
        var newLink = JsonDocument.Parse(@"{
            ""LinkPath"": ""/new"",
            ""State"": ""enabled"",
            ""Redirects"": [
                {
                    ""RedirectUrl"": ""https://example.org"",
                    ""Rules"": [
                        {
                            ""SubjectName"": ""Browser"",
                            ""Predicates"": {
                                ""EqualsTo"": ""Chrome""
                            }
                        }
                    ]
                }
            ]
        }").RootElement;

        var content = new StringContent(newLink.GetRawText(), System.Text.Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/smartlinks", content);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task UpdateSmartLink_ReturnsOk()
    {
        var updatedLink = JsonDocument.Parse(@"{
            ""LinkPath"": ""/test"",
            ""State"": ""disabled"",
            ""Redirects"": [
                {
                    ""RedirectUrl"": ""https://example.org"",
                    ""Rules"": [
                        {
                            ""SubjectName"": ""Browser"",
                            ""Predicates"": {
                                ""EqualsTo"": ""Firefox""
                            }
                        }
                    ]
                }
            ]
        }").RootElement;

        var content = new StringContent(updatedLink.GetRawText(), System.Text.Encoding.UTF8, "application/json");

        var response = await _client.PutAsync("/api/smartlinks", content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteSmartLink_ReturnsOk()
    {
        var path = Uri.EscapeDataString("/test");
        var response = await _client.DeleteAsync($"/api/smartlinks/{path}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteSmartLink_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/api/smartlinks/nonexistent");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
        [Fact]
    public async Task NotFoundMiddleware_ShouldHandleNullFromRepository()
    {
        // Arrange
        var mockRepo = new Mock<IStatableSmartLinkRepository>();
        mockRepo
            .Setup(repo => repo.ReadAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((IStatableSmartLink?)null);

        var factory1 = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(mockRepo.Object);
            });
        });

        var client = factory1.CreateClient();

        // Act
        var response = await client.GetAsync("/non-existing-link");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task NotFoundMiddleware_ShouldHandleDeletedState()
    {
        // Arrange
        var mockLink = new Mock<IStatableSmartLink>();
        mockLink.Setup(link => link.State).Returns("deleted");

        var mockRepo = new Mock<IStatableSmartLinkRepository>();
        mockRepo
            .Setup(repo => repo.ReadAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockLink.Object);

        var factory1 = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(mockRepo.Object);
            });
        });

        var client = factory1.CreateClient();

        // Act
        var response = await client.GetAsync("/deleted-link");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RedirectMiddleware_ShouldPassToNext_WhenNoRedirect()
    {
        // Arrange
        var mockRedirectService = new Mock<IRedirectService>();
        mockRedirectService
            .Setup(service => service.EvaluateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((Uri?)null);

        var factory1 = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(mockRedirectService.Object);
            });

            builder.Configure(app =>
            {
                app.UseMiddleware<RedirectMiddleware>();
                app.Run(async context =>
                {
                    await context.Response.WriteAsync("Next middleware reached");
                });
            });
        });

        var client = factory1.CreateClient();

        // Act
        var response = await client.GetAsync("/some-path");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal("Next middleware reached", content);
    }

    [Fact]
    public async Task RedirectMiddleware_ShouldRedirect_WhenRedirectExists()
    {
        // Arrange
        var mockRedirectService = new Mock<IRedirectService>();
        var redirectUri = new Uri("https://example.com");
        mockRedirectService
            .Setup(service => service.EvaluateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(redirectUri);

        var factory1 = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(mockRedirectService.Object);
            });

            builder.Configure(app =>
            {
                app.UseMiddleware<RedirectMiddleware>();
                app.Run(async context =>
                {
                    await context.Response.WriteAsync("Next middleware reached");
                });
            });
        });

        var client = factory1.CreateClient();

        // Act
        var response = await client.GetAsync("/some-path");

        // Assert
        Assert.Equal(HttpStatusCode.TemporaryRedirect, response.StatusCode);
        Assert.True(response.Headers.Location == redirectUri);
    }
}