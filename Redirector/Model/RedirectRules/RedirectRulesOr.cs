namespace Redirector;

public class RedirectRulesOr(IEnumerable<IRedirectRule> rules) : RedirectRules(rules)
{
    public override bool Evaluate()
    {
        return Rules.Any(r => r.Evaluate());
    }
}