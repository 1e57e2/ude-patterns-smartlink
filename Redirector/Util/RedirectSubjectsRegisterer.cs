using System.Reflection;
using Microsoft.Extensions.Options;

namespace Redirector;

public class RedirectSubjectsRegisterer(IAssemblyLoader assemblyLoader, IOptions<PluginLoaderOptions> options, ILogger logger)
{
    public void AddRedirectSubjects(IServiceCollection services)
    {
        var path = options.Value.ValueProvidersPath;
        if (string.IsNullOrWhiteSpace(path))
        {
            logger.LogError("PluginLoader:ValueProvidersPath is empty");
            return;
        }

        var redirectSubjectInterface = typeof(IRedirectSubject);
        var assemblies = assemblyLoader.Load(path, redirectSubjectInterface);
        if (assemblies.Count == 0)
        {
            logger.LogError($"No suitable assemblies found in '{path}");
            return;
        }

        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                var attribute = type.GetCustomAttribute<RedirectSubjectNameAttribute>();
                if (attribute is null) continue;
                services.AddKeyedScoped(redirectSubjectInterface, attribute.SubjectName, type);
                logger.LogInformation($"Redirect subject '{attribute.SubjectName}' was successfully registered.");
            }
        }
    }
}