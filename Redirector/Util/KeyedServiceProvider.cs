namespace Redirector;

public class KeyedServiceProviderWrapper(IServiceProvider serviceProvider) : IKeyedServiceProviderWrapper
{
    public IServiceScope CreateScope() => serviceProvider.CreateScope();
    public T GetRequiredKeyedService<T>(string key) where T : notnull => serviceProvider.GetRequiredKeyedService<T>(key);
    public T? GetKeyedService<T>(string key) where T : notnull => serviceProvider.GetKeyedService<T>(key);
}