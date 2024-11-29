using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Redirector.Tests;

public class FileStorageRepositoryCacheTests
{
    [Fact]
    public void Constructor_ShouldInitializeCache_WhenFileIsValid()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<FileStorageRepositoryCache>>();
        var optionsMock = new Mock<IOptions<FileStorageRepositoryOptions>>();
        var filePath = Path.Combine(AppContext.BaseDirectory, "FileStorageRepositoryCacheTests.json");
        var json = @"
        [
            { ""LinkPath"": ""/test1"", ""Target"": ""https://example.com/1"" },
            { ""LinkPath"": ""/test2"", ""Target"": ""https://example.com/2"" }
        ]";
        
        File.WriteAllText(filePath, json);

        optionsMock.Setup(o => o.Value).Returns(new FileStorageRepositoryOptions { FilePath = filePath });

        // Act
        var cache = new FileStorageRepositoryCache(loggerMock.Object, optionsMock.Object);
        var result1 = cache.GetDescription("/test1");
        var result2 = cache.GetDescription("/test2");

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal("https://example.com/1", result1?.GetProperty("Target").GetString());
        Assert.Equal("https://example.com/2", result2?.GetProperty("Target").GetString());
        loggerMock.Verify(
            l => l.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Reading all rules from {filePath}")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        File.Delete(filePath);
    }

    [Fact]
    public void Constructor_ShouldThrowJsonException_WhenJsonIsInvalid()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<FileStorageRepositoryCache>>();
        var optionsMock = new Mock<IOptions<FileStorageRepositoryOptions>>();
        var filePath = "test_invalid.json";
        var invalidJson = "{ invalid json }";
        File.WriteAllText(filePath, invalidJson);

        optionsMock.Setup(o => o.Value).Returns(new FileStorageRepositoryOptions { FilePath = filePath });

        // Act & Assert
        Assert.ThrowsAny<JsonException>(() => new FileStorageRepositoryCache(loggerMock.Object, optionsMock.Object));

        File.Delete(filePath);
    }

    [Fact]
    public void GetDescription_ShouldReturnNull_WhenLinkPathDoesNotExist()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<FileStorageRepositoryCache>>();
        var optionsMock = new Mock<IOptions<FileStorageRepositoryOptions>>();
        var filePath = "test.json";
        var json = "[]";
        File.WriteAllText(filePath, json);

        optionsMock.Setup(o => o.Value).Returns(new FileStorageRepositoryOptions { FilePath = filePath });

        var cache = new FileStorageRepositoryCache(loggerMock.Object, optionsMock.Object);

        // Act
        var result = cache.GetDescription("/non-existent");

        // Assert
        Assert.Null(result);

        File.Delete(filePath);
    }
}
