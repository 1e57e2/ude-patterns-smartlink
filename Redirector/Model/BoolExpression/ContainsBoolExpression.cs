namespace Redirector;

public class ContainsBoolExpression<T>(Func<IEnumerable<T>> subjectValueProvider, T? predicateValue)
    : BoolExpression<IEnumerable<T>, T>(subjectValueProvider, predicateValue)
    where T : IEquatable<T>
{
    public override bool Evaluate()
    {
        return SubjectValue is not null && PredicateValue is not null && SubjectValue.Contains(PredicateValue);
    }
}