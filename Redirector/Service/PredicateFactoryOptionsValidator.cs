namespace Redirector;

using Microsoft.Extensions.Options;

public class PredicateFactoryOptionsValidator : IValidateOptions<PredicateFactoryOptions>
{
    public ValidateOptionsResult Validate(string? name, PredicateFactoryOptions options)
    {
        foreach (var (operationName, expressionType) in options.Operations)
        {
            if (expressionType.IsInterface || expressionType.IsAbstract)
            {
                return ValidateOptionsResult.Fail(
                    $"Unable to register operation '{operationName}'. Expression type '{expressionType.Name}' is an interface or abstract class.");
            }

            if (!typeof(IRedirectRule).IsAssignableFrom(expressionType))
            {
                return ValidateOptionsResult.Fail(
                    $"Unable to register operation '{operationName}'. Expression type '{expressionType.Name}' should implement '{nameof(IRedirectRule)}'.");
            }
        }

        return ValidateOptionsResult.Success;
    }
}
