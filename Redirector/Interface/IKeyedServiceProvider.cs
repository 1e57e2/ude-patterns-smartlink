namespace Redirector;

public interface IKeyedServiceProviderWrapper
{
    IServiceScope CreateScope();
    T GetRequiredKeyedService<T>(string key) where T : notnull;
    T? GetKeyedService<T>(string key) where T : notnull;
}