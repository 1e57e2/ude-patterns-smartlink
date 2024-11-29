namespace Redirector;

public abstract class BoolExpression<TSubject, TPredicate>(
    Func<TSubject?> subjectValueProvider,
    TPredicate? predicateValue)
    : IRedirectRule
{
    private readonly Lazy<TSubject?> _subjectValueCache = new(subjectValueProvider.Invoke);
    protected TSubject? SubjectValue => _subjectValueCache.Value;
    protected TPredicate? PredicateValue { get; } = predicateValue;

    public abstract bool Evaluate();
}