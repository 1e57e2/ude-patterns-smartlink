using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Redirector.Tests;

public class DbStorageRepositoryTests
{

    [Fact]
    public async Task Should_ReturnDescription_WhenLinkPathIsValid()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<RedirectsDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        await using var context = new RedirectsDbContext(options);
        context.SmartLinkDescription.Add(new SmartLinkDescription
        {
            LinkPath = "/test",
            Description = JsonDocument.Parse("{\"Key\":\"Value\"}").RootElement
        });
        await context.SaveChangesAsync();

        var smartLinkMock = new Mock<ISmartLink>();
        smartLinkMock.Setup(s => s.Path).Returns("/test");

        var repository = new DbStorageRepository(context, smartLinkMock.Object);

        // Act
        var description = await repository.GetDescriptionAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(description);
        Assert.Equal("Value", description?.GetProperty("Key").GetString());
    }
}
