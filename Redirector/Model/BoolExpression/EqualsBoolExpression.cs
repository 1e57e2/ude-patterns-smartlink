namespace Redirector;

public class EqualsBoolExpression<T>(Func<T?> subjectValueProvider, T? predicateValue)
    : BoolExpression<T, T>(subjectValueProvider, predicateValue)
//where T : IEquatable<T>
{
    public override bool Evaluate()
    {
        return PredicateValue is not null && SubjectValue is not null && SubjectValue.Equals(PredicateValue);
    }
}