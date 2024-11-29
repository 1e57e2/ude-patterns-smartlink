namespace Redirector;

public abstract class RedirectRules(IEnumerable<IRedirectRule> rules) : IRedirectRule
{
    protected IEnumerable<IRedirectRule> Rules { get; } = rules;
    public abstract bool Evaluate();
}