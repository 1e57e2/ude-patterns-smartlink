using System.Text.Json;
using Moq;

namespace Redirector.Tests;

public class StatableSmartLinkRepositoryTests
{
    [Fact]
    public async Task ReadAsync_ShouldReturnStatableSmartLink_WhenValidJsonProvided()
    {
        // Arrange
        var validJson = @"
        {
            ""State"": ""enabled""
        }";

        var jsonElement = JsonDocument.Parse(validJson).RootElement;

        var storageRepositoryMock = new Mock<IStorageRepository>();
        storageRepositoryMock
            .Setup(repo => repo.GetDescriptionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(jsonElement);

        var repository = new StatableSmartLinkRepository(storageRepositoryMock.Object);

        // Act
        var result = await repository.ReadAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("enabled", result!.State);

        storageRepositoryMock.Verify(repo => repo.GetDescriptionAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ReadAsync_ShouldReturnNull_WhenJsonIsNull()
    {
        // Arrange
        var storageRepositoryMock = new Mock<IStorageRepository>();
        storageRepositoryMock
            .Setup(repo => repo.GetDescriptionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((JsonElement?)null);

        var repository = new StatableSmartLinkRepository(storageRepositoryMock.Object);

        // Act
        var result = await repository.ReadAsync(CancellationToken.None);

        // Assert
        Assert.Null(result);

        storageRepositoryMock.Verify(repo => repo.GetDescriptionAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ReadAsync_ShouldReturnNull_WhenJsonIsInvalid()
    {
        // Arrange
        var invalidJson = @"
        {
            ""InvalidProperty"": ""value""
        }";

        var jsonElement = JsonDocument.Parse(invalidJson).RootElement;

        var storageRepositoryMock = new Mock<IStorageRepository>();
        storageRepositoryMock
            .Setup(repo => repo.GetDescriptionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(jsonElement);

        var repository = new StatableSmartLinkRepository(storageRepositoryMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<JsonException>(async () => await repository.ReadAsync(default));
    }
}
