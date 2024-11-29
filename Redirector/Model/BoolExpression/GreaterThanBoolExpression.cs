namespace Redirector;

public class GreaterThanBoolExpression<T>(Func<T?> subjectValueProvider, T? predicateValue)
    : BoolExpression<T, T>(subjectValueProvider, predicateValue)
    where T : IComparable<T>
{
    public override bool Evaluate()
    {
        return PredicateValue is not null && SubjectValue is not null && SubjectValue.CompareTo(PredicateValue) > 0;
    }
}