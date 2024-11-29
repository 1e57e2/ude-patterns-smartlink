namespace Redirector.Tests;

public class RedirectRulesTests
{
    [Fact]
    public void EqualsBoolExpression_ShouldReturnTrue_WhenSubjectEqualsPredicate()
    {
        // Arrange
        var subjectProvider = () => "Test";
        var predicate = "Test";
        var expression = new EqualsBoolExpression<string>(subjectProvider, predicate);

        // Act
        var result = expression.Evaluate();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void EqualsBoolExpression_ShouldReturnFalse_WhenSubjectDoesNotEqualPredicate()
    {
        // Arrange
        var subjectProvider = () => "Test";
        var predicate = "Other";
        var expression = new EqualsBoolExpression<string>(subjectProvider, predicate);

        // Act
        var result = expression.Evaluate();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void LessThanBoolExpression_ShouldReturnTrue_WhenSubjectIsLessThanPredicate()
    {
        // Arrange
        var subjectProvider = () => 5;
        var predicate = 10;
        var expression = new LessThanBoolExpression<int>(subjectProvider, predicate);

        // Act
        var result = expression.Evaluate();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void GreaterThanBoolExpression_ShouldReturnTrue_WhenSubjectIsGreaterThanPredicate()
    {
        // Arrange
        var subjectProvider = () => 15;
        var predicate = 10;
        var expression = new GreaterThanBoolExpression<int>(subjectProvider, predicate);

        // Act
        var result = expression.Evaluate();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsInBoolExpression_ShouldReturnTrue_WhenSubjectIsInPredicateCollection()
    {
        // Arrange
        var subjectProvider = () => "Item";
        var predicate = new List<string> { "Item", "Other" };
        var expression = new IsInBoolExpression<string>(subjectProvider, predicate);

        // Act
        var result = expression.Evaluate();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ContainsBoolExpression_ShouldReturnTrue_WhenPredicateIsInSubjectCollection()
    {
        // Arrange
        var subjectProvider = () => new List<string> { "Item", "Other" };
        var predicate = "Item";
        var expression = new ContainsBoolExpression<string>(subjectProvider, predicate);

        // Act
        var result = expression.Evaluate();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void RedirectRulesAnd_ShouldReturnTrue_WhenAllNestedRulesEvaluateTrue()
    {
        // Arrange
        var rules = new List<IRedirectRule>
        {
            new EqualsBoolExpression<string>(() => "Test", "Test"),
            new GreaterThanBoolExpression<int>(() => 15, 10)
        };
        var andRule = new RedirectRulesAnd(rules);

        // Act
        var result = andRule.Evaluate();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void RedirectRulesAnd_ShouldReturnFalse_WhenAnyNestedRuleEvaluatesFalse()
    {
        // Arrange
        var rules = new List<IRedirectRule>
        {
            new EqualsBoolExpression<string>(() => "Test", "Test"),
            new GreaterThanBoolExpression<int>(() => 5, 10)
        };
        var andRule = new RedirectRulesAnd(rules);

        // Act
        var result = andRule.Evaluate();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void RedirectRulesOr_ShouldReturnTrue_WhenAnyNestedRuleEvaluatesTrue()
    {
        // Arrange
        var rules = new List<IRedirectRule>
        {
            new EqualsBoolExpression<string>(() => "Test", "Other"),
            new GreaterThanBoolExpression<int>(() => 15, 10)
        };
        var orRule = new RedirectRulesOr(rules);

        // Act
        var result = orRule.Evaluate();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void RedirectRulesOr_ShouldReturnFalse_WhenAllNestedRulesEvaluateFalse()
    {
        // Arrange
        var rules = new List<IRedirectRule>
        {
            new EqualsBoolExpression<string>(() => "Test", "Other"),
            new GreaterThanBoolExpression<int>(() => 5, 10)
        };
        var orRule = new RedirectRulesOr(rules);

        // Act
        var result = orRule.Evaluate();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void RedirectRulesOr_ShouldWorkWithNestedAndRules()
    {
        // Arrange
        var nestedAndRules = new RedirectRulesAnd(new List<IRedirectRule>
        {
            new EqualsBoolExpression<string>(() => "Test", "Test"),
            new GreaterThanBoolExpression<int>(() => 15, 10)
        });

        var orRule = new RedirectRulesOr(new List<IRedirectRule>
        {
            nestedAndRules,
            new EqualsBoolExpression<string>(() => "Test", "Other")
        });

        // Act
        var result = orRule.Evaluate();

        // Assert
        Assert.True(result);
    }
}