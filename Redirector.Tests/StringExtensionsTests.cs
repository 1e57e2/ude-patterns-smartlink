using System.Net;

namespace Redirector.Tests;

public class StringExtensionsTests
{
    public static readonly object[][] TryParseCases =
    [
        ["123", typeof(int), 123],
        ["123.45", typeof(float), 123.45f],
        ["123.45", typeof(double), 123.45d],
        ["true", typeof(bool), true],
        ["2023-11-25", typeof(DateTime), new DateTime(2023, 11, 25)]
    ];      
    [Theory, MemberData(nameof(TryParseCases))]
    public void TryParse_ShouldReturnTrue_WhenUsingTryParseMethod(string input, Type targetType, object expected)
    {
        // Act
        var success = input.TryParse(targetType, out var result);

        // Assert
        Assert.True(success);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void TryParse_ShouldReturnTrue_WhenTargetTypeIsString()
    {
        // Arrange
        var input = "test";

        // Act
        var success = input.TryParse(typeof(string), out var result);

        // Assert
        Assert.True(success);
        Assert.Equal(input, result);
    }

    [Theory]
    [InlineData("invalid", typeof(int))]
    [InlineData("not-a-date", typeof(DateTime))]
    [InlineData("123.45", typeof(int))]
    public void TryParse_ShouldReturnFalse_WhenConversionFails(string input, Type targetType)
    {
        // Act
        var success = input.TryParse(targetType, out var result);

        // Assert
        Assert.False(success);
        Assert.Null(result);
    }

    [Fact]
    public void TryParse_ShouldReturnFalse_WhenNoTryParseOrParseMethodExists()
    {
        // Arrange
        var input = "test";

        // Act
        var success = input.TryParse(typeof(StringExtensionsTests), out var result);

        // Assert
        Assert.False(success);
        Assert.Null(result);
    }
}
