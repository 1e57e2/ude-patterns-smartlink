using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Redirector.Tests;

public class SmartLinkEditorServiceTests
{
    private readonly DbContextOptions<RedirectsDbContext> _dbContextOptions = new DbContextOptionsBuilder<RedirectsDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

    [Fact]
    public async Task CreateSmartLinkAsync_ShouldReturnTrue_WhenLinkDoesNotExist()
    {
        // Arrange
        using var context = new RedirectsDbContext(_dbContextOptions);
        var service = new SmartLinkEditorService(context);

        var newLink = new SmartLinkDescription
        {
            LinkPath = "/test",
            Description = JsonDocument.Parse("{\"State\":\"enabled\"}").RootElement
        };

        // Act
        var result = await service.CreateSmartLinkAsync(newLink, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Single(context.SmartLinkDescription);
    }

    [Fact]
    public async Task CreateSmartLinkAsync_ShouldReturnFalse_WhenLinkExists()
    {
        // Arrange
        using var context = new RedirectsDbContext(_dbContextOptions);
        context.SmartLinkDescription.Add(new SmartLinkDescription
        {
            LinkPath = "/test",
            Description = JsonDocument.Parse("{\"State\":\"enabled\"}").RootElement
        });
        await context.SaveChangesAsync();

        var service = new SmartLinkEditorService(context);

        var duplicateLink = new SmartLinkDescription
        {
            LinkPath = "/test",
            Description = JsonDocument.Parse("{\"State\":\"enabled\"}").RootElement
        };

        // Act
        var result = await service.CreateSmartLinkAsync(duplicateLink, CancellationToken.None);

        // Assert
        Assert.False(result);
        Assert.Single(context.SmartLinkDescription);
    }

    [Fact]
    public async Task GetSmartLinks_ShouldReturnAllLinks()
    {
        // Arrange
        using var context = new RedirectsDbContext(_dbContextOptions);
        context.SmartLinkDescription.AddRange(
            new SmartLinkDescription
            {
                LinkPath = "/link1",
                Description = JsonDocument.Parse("{\"State\":\"enabled\"}").RootElement
            },
            new SmartLinkDescription
            {
                LinkPath = "/link2",
                Description = JsonDocument.Parse("{\"State\":\"disabled\"}").RootElement
            });
        await context.SaveChangesAsync();

        var service = new SmartLinkEditorService(context);

        // Act
        var links = await service.GetSmartLinks(CancellationToken.None);

        // Assert
        Assert.Equal(2, links.Count);
    }

    [Fact]
    public async Task GetSmartLinks_ShouldReturnSpecificLink()
    {
        // Arrange
        using var context = new RedirectsDbContext(_dbContextOptions);
        context.SmartLinkDescription.Add(new SmartLinkDescription
        {
            LinkPath = "/test",
            Description = JsonDocument.Parse("{\"State\":\"enabled\"}").RootElement
        });
        await context.SaveChangesAsync();

        var service = new SmartLinkEditorService(context);

        // Act
        var link = await service.GetSmartLinks("/test", CancellationToken.None);

        // Assert
        Assert.NotNull(link);
        Assert.Equal("/test", link!.LinkPath);
    }

    [Fact]
    public async Task UpdateSmartLinkAsync_ShouldReturnTrue_WhenLinkExists()
    {
        // Arrange
        using var context = new RedirectsDbContext(_dbContextOptions);
        context.SmartLinkDescription.Add(new SmartLinkDescription
        {
            LinkPath = "/test",
            Description = JsonDocument.Parse("{\"State\":\"enabled\"}").RootElement
        });
        await context.SaveChangesAsync();

        var service = new SmartLinkEditorService(context);

        var updatedLink = new SmartLinkDescription
        {
            LinkPath = "/test",
            Description = JsonDocument.Parse("{\"State\":\"disabled\"}").RootElement
        };

        // Act
        var result = await service.UpdateSmartLinkAsync(updatedLink, CancellationToken.None);

        // Assert
        Assert.True(result);
        var updated = await context.SmartLinkDescription.FirstAsync();
        Assert.Equal("{\"State\":\"disabled\"}", updated.Description.GetRawText());
    }

    [Fact]
    public async Task DeleteSmartLinkAsync_ShouldReturnTrue_WhenLinkExists()
    {
        // Arrange
        using var context = new RedirectsDbContext(_dbContextOptions);
        context.SmartLinkDescription.Add(new SmartLinkDescription
        {
            LinkPath = "/test",
            Description = JsonDocument.Parse("{\"State\":\"enabled\"}").RootElement
        });
        await context.SaveChangesAsync();

        var service = new SmartLinkEditorService(context);

        // Act
        var result = await service.DeleteSmartLinkAsync("/test", CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Empty(context.SmartLinkDescription);
    }

    [Fact]
    public async Task DeleteSmartLinkAsync_ShouldReturnFalse_WhenLinkDoesNotExist()
    {
        // Arrange
        using var context = new RedirectsDbContext(_dbContextOptions);
        var service = new SmartLinkEditorService(context);

        // Act
        var result = await service.DeleteSmartLinkAsync("/nonexistent", CancellationToken.None);

        // Assert
        Assert.False(result);
    }
}