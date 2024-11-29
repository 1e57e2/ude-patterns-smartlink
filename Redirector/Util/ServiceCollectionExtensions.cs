namespace Redirector;

public static class ServiceCollectionExtensions
{
    public static void AddRedirectSubjectsFromAssemblies(this IServiceCollection services, IConfiguration configuration)
    {
        var tempServices = new ServiceCollection();
    
        tempServices.Configure<PluginLoaderOptions>(configuration.GetSection("PluginLoader"));
    
        using var loggerFactory = LoggerFactory.Create(loggingBuilder =>
        {
            loggingBuilder.AddConsole();
            loggingBuilder.AddConfiguration(configuration.GetSection("Logging"));
        });
        tempServices.AddSingleton<ILogger>(loggerFactory.CreateLogger<Program>());
        tempServices.AddSingleton<IFileSystem, FileSystem>();
        tempServices.AddSingleton<IAssemblyLoader, AssemblyLoader>();
    
        tempServices.AddSingleton<RedirectSubjectsRegisterer>();
    
        // suppressing ASP0000 as we're using a disposable temporary container
#pragma warning disable ASP0000
        using var tempServiceProvider = tempServices.BuildServiceProvider();
#pragma warning restore ASP0000
    
        var serviceProviderRegistrator = tempServiceProvider.GetRequiredService<RedirectSubjectsRegisterer>();
        serviceProviderRegistrator.AddRedirectSubjects(services);
    }
}