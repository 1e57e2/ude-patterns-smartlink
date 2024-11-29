namespace Redirector;

public interface IPredicateFactory
{
    IRedirectRule CreatePredicate(string subjectName, string operation, object value);
}