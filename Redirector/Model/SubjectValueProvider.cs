namespace Redirector;

public abstract class SubjectValueProvider<T> : IRedirectSubject
{
    protected abstract T? Value { get; }
    public object ValueProvider => () => Value;
}

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class RedirectSubjectNameAttribute(string subjectName) : Attribute
{
    public string SubjectName { get; } = subjectName;
}