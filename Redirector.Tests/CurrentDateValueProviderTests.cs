using Provider = CurrentDateValueProvider;

namespace Redirector.Tests;

public class CurrentDateValueProviderTests
{
    [Fact]
    public void Value_ShouldReturnCurrentUtcDateTime()
    {
        // Arrange
        var provider = new Provider.CurrentDateValueProvider();
        var getValue = (Func<DateTime>)provider.ValueProvider;

        // Act
        var before = DateTime.UtcNow;
        var value = getValue();
        var after = DateTime.UtcNow;

        // Assert
        Assert.True(value >= before && value <= after,
            $"Value {value} is not between {before} and {after}");
    }
    
    [Fact]
    public void Value_ShouldReturnUtcKind()
    {
        // Arrange
        var provider = new Provider.CurrentDateValueProvider();
        var getValue = (Func<DateTime>)provider.ValueProvider;

        // Act
        var value = getValue();

        // Assert
        Assert.Equal(DateTimeKind.Utc, value.Kind);
    }
}