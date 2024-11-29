namespace Redirector;

public class IsInBoolExpression<T>(Func<T?> subjectValueProvider, IEnumerable<T>? predicateValue)
    : BoolExpression<T, IEnumerable<T>>(subjectValueProvider, predicateValue)
    where T : IEquatable<T>
{
    public override bool Evaluate()
    {
        return SubjectValue is not null && PredicateValue != null && PredicateValue.Contains(SubjectValue);
    }
}