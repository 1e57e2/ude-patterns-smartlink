namespace Redirector.Tests;

public class EqualsBoolExpressionTests
{
    [Fact]
    public void ShouldReturnTrue_WhenValuesAreEqual()
    {
        // Arrange
        var subjectProvider = () => "Chrome";
        var predicateValue = "Chrome";

        var expression = new EqualsBoolExpression<string>(subjectProvider, predicateValue);

        // Act
        var result = expression.Evaluate();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ShouldReturnFalse_WhenValuesAreNotEqual()
    {
        // Arrange
        var subjectProvider = () => "Firefox";
        var predicateValue = "Chrome";

        var expression = new EqualsBoolExpression<string>(subjectProvider, predicateValue);

        // Act
        var result = expression.Evaluate();

        // Assert
        Assert.False(result);
    }
}
