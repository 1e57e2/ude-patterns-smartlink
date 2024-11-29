using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Redirector.Tests;

public class RedirectsRepositoryTests
{
    private readonly Mock<IStorageRepository> _storageRepositoryMock = new();
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private RedirectsRepository CreateRepository() => new(_storageRepositoryMock.Object, _serializerOptions);

    [Fact]
    public async Task ReadAsync_ShouldParseJsonAndReturnIRedirectCollection()
    {
        // Arrange
        var validJson = @"
        {
            ""Redirects"": [
                {
                ""RedirectUrl"": ""https://example.org"",
                ""Rules"": [
                  {
                    ""Operation"": ""Or"",
                    ""Rules"": [
                      {
                        ""SubjectName"": ""CurrentDate"",
                        ""Predicates"": {
                          ""GreaterThan"": ""2024-11-20"",
                          ""LessThan"": ""2024-11-22""
                        }
                      },
                      {
                        ""SubjectName"": ""Browser"",
                        ""Predicates"": {
                          ""EqualsTo"": ""Chrome""
                        }
                      }
                    ]
                  }
                ]
                },
                {
                ""RedirectUrl"": ""https://example.com"",
                ""Rules"": [
                  {
                    ""Operation"": ""And"",
                    ""Rules"": [
                      {
                        ""SubjectName"": ""CurrentDate"",
                        ""Predicates"": {
                          ""GreaterThan"": ""2024-11-21""
                        }
                      },
                      {
                        ""SubjectName"": ""Browser"",
                        ""Predicates"": {
                          ""EqualsTo"": ""Firefox""
                        }
                      }
                    ]
                  }
                ]
                }
                ]
        }";

        var jsonElement = JsonDocument.Parse(validJson).RootElement;
        _storageRepositoryMock
            .Setup(repo => repo.GetDescriptionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(jsonElement);

        var mockKeyedServiceProvider = new Mock<IKeyedServiceProviderWrapper>();

        mockKeyedServiceProvider
            .Setup(provider => provider.GetRequiredKeyedService<IRedirectRuleDeserializer>("Or"))
            .Returns(new OrRuleDeserializer());
        
        mockKeyedServiceProvider
            .Setup(provider => provider.GetRequiredKeyedService<IRedirectRuleDeserializer>("And"))
            .Returns(new AndRuleDeserializer());

        mockKeyedServiceProvider
            .Setup(provider => provider.GetRequiredKeyedService<IRedirectRuleDeserializer>("Predicates"))
            .Returns(new PredicatesRuleDeserializer(new Mock<IPredicateFactory>().Object));

        var serializerOptions = new JsonSerializerOptions
        {
            Converters = { new ObjectConverter(), new RedirectRuleConverter(mockKeyedServiceProvider.Object) }
        };

        var repository = new RedirectsRepository(_storageRepositoryMock.Object, serializerOptions);

        // Act
        var redirects = await repository.ReadAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(redirects);
        Assert.Equal(2, redirects.Count);

        var redirect = redirects.First();
        Assert.Equal("https://example.org/", redirect.RedirectUrl.ToString());
        Assert.Single(redirect.Rules);

        var rule = redirect.Rules.FirstOrDefault();
        Assert.NotNull(rule);
        Assert.IsType<RedirectRulesOr>(rule);
    }

    [Fact]
    public async Task ReadAsync_ShouldReturnEmptyCollection_WhenJsonIsInvalid()
    {
        // Arrange
        var invalidJson = @"
        {
            ""InvalidProperty"": [
                { ""Something"": ""Wrong"" }
            ]
        }";

        var jsonElement = JsonDocument.Parse(invalidJson).RootElement;
        _storageRepositoryMock
            .Setup(repo => repo.GetDescriptionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(jsonElement);

        var repository = CreateRepository();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<JsonException>(async () => await repository.ReadAsync(default));
        Assert.Equal("JSON deserialization for type 'Redirector.RedirectableSmartLink' was missing required properties including: 'Redirects'.", exception.Message);
    }
}
