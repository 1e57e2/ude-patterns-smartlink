using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Redirector.Tests;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddSingleton(Mock.Of<IStatableSmartLinkRepository>());
            services.AddSingleton(Mock.Of<IRedirectService>());
            
            var mockSmartLinkEditorService = new Mock<ISmartLinkEditorService>();
            var mockSmartLinkValidator = new Mock<ISmartLinkDescrptionValidator>();

            var sampleSmartLink = new SmartLinkDescription
            {
                LinkPath = "/test",
                Description = JsonDocument.Parse(@"{
                    ""LinkPath"": ""/test"",
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
                }").RootElement
            };

            // Настройка заглушек
            mockSmartLinkEditorService
                .Setup(s => s.GetSmartLinks(default))
                .ReturnsAsync(new List<SmartLinkDescription> { sampleSmartLink });

            mockSmartLinkEditorService
                .Setup(s => s.GetSmartLinks(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((string path, CancellationToken _) => path == "/test" ? sampleSmartLink : null);

            mockSmartLinkEditorService
                .Setup(s => s.CreateSmartLinkAsync(It.IsAny<SmartLinkDescription>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            mockSmartLinkEditorService
                .Setup(s => s.UpdateSmartLinkAsync(It.IsAny<SmartLinkDescription>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            mockSmartLinkEditorService
                .Setup(s => s.DeleteSmartLinkAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((string path, CancellationToken _) => path == "/test");

            mockSmartLinkValidator
                .Setup(v => v.TryGetDescription(It.IsAny<JsonElement>(), out It.Ref<SmartLinkDescription>.IsAny, out It.Ref<string>.IsAny!))
                .Returns((JsonElement input, out SmartLinkDescription desc, out string message) =>
                {
                    desc = new SmartLinkDescription
                    {
                        LinkPath = input.GetProperty("LinkPath").GetString()!,
                        Description = input
                    };
                    message = string.Empty;
                    return true;
                });

            services.AddSingleton(mockSmartLinkEditorService.Object);
            services.AddSingleton(mockSmartLinkValidator.Object);
        });
    }
}