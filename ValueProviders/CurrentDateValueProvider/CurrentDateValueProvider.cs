using Redirector;

namespace CurrentDateValueProvider;

[RedirectSubjectName("CurrentDate")]
public class CurrentDateValueProvider : SubjectValueProvider<DateTime>
{
    protected override DateTime Value => DateTime.UtcNow;
}