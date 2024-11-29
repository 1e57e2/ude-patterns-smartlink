using Microsoft.Extensions.Options;
using System.Reflection;

namespace Redirector;

public class PredicateFactory(IKeyedServiceProviderWrapper serviceProviderWrapper, IOptions<PredicateFactoryOptions> options) : IPredicateFactory
{
    private readonly Dictionary<string, Type> _operationRegistry = options.Value.Operations;
    private readonly Dictionary<string, Type> _subjectTypeCache = new();

    public IRedirectRule CreatePredicate(string subjectName, string operation, object value)
    {
        using var scope = serviceProviderWrapper.CreateScope();
        var subject = scope.ServiceProvider.GetKeyedService<IRedirectSubject>(subjectName);
        if (subject is null)
            throw new InvalidRedirectRuleException($"Value provider for {subjectName} not found");

        if (!_subjectTypeCache.TryGetValue(subjectName, out var subjectValueType))
        {
            var valueProviderType = subject.ValueProvider.GetType();
            subjectValueType = valueProviderType.GetGenericArguments().First();
            _subjectTypeCache.Add(subjectName, subjectValueType);
        }

        if (!_operationRegistry.TryGetValue(operation, out var predicateType))
            throw new InvalidRedirectRuleException($"Unknown predicate operation '{operation}'.");

        if (predicateType.IsGenericTypeDefinition)
            predicateType = predicateType.MakeGenericType(subjectValueType);

        var predicateValue = ConvertPredicateValue(value, subjectValueType);

        var predicate = (IRedirectRule)Activator.CreateInstance(predicateType, subject.ValueProvider, predicateValue)!;

        return predicate;
    }

    private object ConvertPredicateValue(object value, Type targetType)
    {
        if (targetType != typeof(string) && value is string stringValue)
        {
            if (!stringValue.TryParse(targetType, out var parsedValue))
                throw new InvalidRedirectRuleException($"Cannot parse '{stringValue}' to type '{targetType.Name}'.");
            return parsedValue!;
        }

        if (value is IEnumerable<object> array)
            return ConvertToTypedList(array, targetType);

        return value;
    }

    private object ConvertToTypedList(IEnumerable<object> array, Type targetType)
    {
        return typeof(Enumerable)
            .GetMethod("Cast", BindingFlags.Static | BindingFlags.Public)!
            .MakeGenericMethod(targetType)
            .Invoke(null, [array])!;
    }
}