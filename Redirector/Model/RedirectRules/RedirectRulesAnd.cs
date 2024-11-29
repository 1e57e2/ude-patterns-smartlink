namespace Redirector;

public class RedirectRulesAnd(IEnumerable<IRedirectRule> rules) : RedirectRules(rules)
{
    public override bool Evaluate()
    {
        return Rules.All(r => r.Evaluate());
    }
}