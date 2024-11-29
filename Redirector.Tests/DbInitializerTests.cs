using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Redirector.Tests;

public class DbInitializerTests
{
    private RedirectsDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<RedirectsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Уникальная база для каждого теста
            .Options;

        return new RedirectsDbContext(options);
    }

    [Fact]
    public void Seed_ShouldAddData_WhenDatabaseIsEmpty()
    {
        // Arrange
        var dbContext = CreateInMemoryDbContext();
        var sourceFilePath = Path.Combine(AppContext.BaseDirectory, "DbInitializerTests.json");
        var json = @"
        [
            { ""LinkPath"": ""/test1"", ""State"": ""enabled"", ""Redirects"": [{""RedirectUrl"": ""https://example.com"" }] },
            { ""LinkPath"": ""/test2"", ""State"": ""enabled"", ""Redirects"": [{""RedirectUrl"": ""https://example.com"" }] }
        ]";
        File.WriteAllText(sourceFilePath, json);

        // Act
        DbInitializer.Seed(dbContext, sourceFilePath);

        // Assert
        var data = dbContext.SmartLinkDescription.ToList();
        Assert.Equal(2, data.Count);
        Assert.Equal("/test1", data[0].LinkPath);
        Assert.Equal("/test2", data[1].LinkPath);

        File.Delete(sourceFilePath);
    }

    [Fact]
    public void Seed_ShouldNotAddData_WhenDatabaseIsNotEmpty()
    {
        // Arrange
        var dbContext = CreateInMemoryDbContext();
        dbContext.SmartLinkDescription.Add(new SmartLinkDescription
        {
            LinkPath = "/existing",
            Description = JsonDocument.Parse(@"[{""RedirectUrl"": ""https://example.org"" }]").RootElement
        });
        dbContext.SaveChanges();

        var sourceFilePath = "test.json";
        var json = @"
        [
            { ""LinkPath"": ""/test1"", ""State"": ""enabled"", ""Redirects"": [{""RedirectUrl"": ""https://example.com"" }] }
        ]";
        File.WriteAllText(sourceFilePath, json);

        // Act
        DbInitializer.Seed(dbContext, sourceFilePath);

        // Assert
        var data = dbContext.SmartLinkDescription.ToList();
        Assert.Single(data);
        Assert.Equal("/existing", data[0].LinkPath);

        File.Delete(sourceFilePath);
    }

    [Fact]
    public void Seed_ShouldThrowJsonException_WhenLinkPathIsMissing()
    {
        // Arrange
        var dbContext = CreateInMemoryDbContext();
        var sourceFilePath = "test_invalid.json";
        var invalidJson = @"
        [
            { ""State"": ""enabled"", ""Redirects"": [{""RedirectUrl"": ""https://example.com"" }] }
        ]";
        File.WriteAllText(sourceFilePath, invalidJson);

        // Act & Assert
        var exception = Assert.Throws<KeyNotFoundException>(() =>
            DbInitializer.Seed(dbContext, sourceFilePath)
        );

        File.Delete(sourceFilePath);
    }
}
