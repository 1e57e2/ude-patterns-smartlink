namespace Redirector.Tests;

public class PredicateFactoryOptionsValidatorTests
{
    [Fact]
    public void ShouldReturnFail_WhenOperationIsNotIRedirectRule()
    {
        // Arrange
        var options = new PredicateFactoryOptions();
        options.Operations.Add("InvalidOperation", typeof(object));

        var validator = new PredicateFactoryOptionsValidator();

        // Act
        var result = validator.Validate(null, options);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains("Expression type 'Object' should implement 'IRedirectRule'", result.FailureMessage);
    }

    [Fact]
    public void ShouldReturnFail_WhenOperationIsAbstract()
    {
        // Arrange
        var options = new PredicateFactoryOptions();
        options.Operations.Add("AbstractOperation", typeof(AbstractRedirectRule));

        var validator = new PredicateFactoryOptionsValidator();

        // Act
        var result = validator.Validate(null, options);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains("Expression type 'AbstractRedirectRule' is an interface or abstract class", result.FailureMessage);
    }

    [Fact]
    public void ShouldReturnSuccess_WhenOperationsAreValid()
    {
        // Arrange
        var options = new PredicateFactoryOptions();
        options.Operations.Add("EqualsTo", typeof(EqualsBoolExpression<>));
        options.Operations.Add("GreaterThan", typeof(GreaterThanBoolExpression<>));

        var validator = new PredicateFactoryOptionsValidator();

        // Act
        var result = validator.Validate(null, options);

        // Assert
        Assert.True(result.Succeeded);
    }

    private abstract class AbstractRedirectRule : IRedirectRule
    {
        public bool Evaluate() => false;
    }
}