using System.Text.Json;
using Moq;

namespace Redirector.Tests;

public class FileStorageRepositoryTests
{
    private readonly Mock<IFileStorageRepositoryCache> _mockCache = new();
    private readonly Mock<ISmartLink> _mockSmartLink = new();

    [Fact]
    public async Task GetDescriptionAsync_ShouldReturnDescription_WhenPathIsFoundInCache()
    {
        // Arrange
        var testPath = "/test-path";
        var expectedDescription = JsonDocument.Parse("{\"key\":\"value\"}").RootElement;

        _mockSmartLink.Setup(s => s.Path).Returns(testPath);
        _mockCache.Setup(c => c.GetDescription(testPath)).Returns(expectedDescription);

        var repository = new FileStorageRepository(_mockCache.Object, _mockSmartLink.Object);

        // Act
        var result = await repository.GetDescriptionAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedDescription, result);
    }

    [Fact]
    public async Task GetDescriptionAsync_ShouldReturnNull_WhenPathIsNotFoundInCache()
    {
        // Arrange
        var testPath = "/non-existing-path";

        _mockSmartLink.Setup(s => s.Path).Returns(testPath);
        _mockCache.Setup(c => c.GetDescription(testPath)).Returns((JsonElement?)null);

        var repository = new FileStorageRepository(_mockCache.Object, _mockSmartLink.Object);

        // Act
        var result = await repository.GetDescriptionAsync(CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetDescriptionAsync_ShouldReturnNull_WhenPathIsNull()
    {
        // Arrange
        _mockSmartLink.Setup(s => s.Path).Returns((string?)null);
        _mockCache.Setup(c => c.GetDescription(null)).Returns((JsonElement?)null);

        var repository = new FileStorageRepository(_mockCache.Object, _mockSmartLink.Object);

        // Act
        var result = await repository.GetDescriptionAsync(CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetDescriptionAsync_ShouldCallGetDescription_WithCorrectPath()
    {
        // Arrange
        var testPath = "/test-path";

        _mockSmartLink.Setup(s => s.Path).Returns(testPath);

        var repository = new FileStorageRepository(_mockCache.Object, _mockSmartLink.Object);

        // Act
        await repository.GetDescriptionAsync(CancellationToken.None);

        // Assert
        _mockCache.Verify(c => c.GetDescription(testPath), Times.Once);
    }
}