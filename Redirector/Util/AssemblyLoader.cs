using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

namespace Redirector;

public class AssemblyLoader(IFileSystem fileSystem) : IAssemblyLoader
{
    public IReadOnlyCollection<Assembly> Load(string directoryPath, Type type)
    {
        var currentAssemblyPath = AppDomain.CurrentDomain.BaseDirectory;
        var fullDirectoryPath = Path.Combine(currentAssemblyPath, directoryPath);
        var assemblyPaths = fileSystem.GetFiles(fullDirectoryPath, "*.dll").ToArray();
        var runtimeAssemblyPaths = Directory.GetFiles(RuntimeEnvironment.GetRuntimeDirectory(), "*.dll")
            .Concat([type.Assembly.Location])
            .Concat(assemblyPaths);
        var resolver = new PathAssemblyResolver(runtimeAssemblyPaths);

        using var metadataContext = new MetadataLoadContext(resolver);
        var mType = metadataContext.LoadFromAssemblyName(type.Assembly.GetName()).GetType(type.FullName!);
        Debug.Assert(mType is not null, nameof(mType) + " is not null");

        var loadedAssemblies = assemblyPaths
            .Select(file => metadataContext.LoadFromAssemblyPath(file))
            .Where(assembly => assembly.GetTypes().Any(t => mType.IsAssignableFrom(t)))
            .Select(assembly => AssemblyLoadContext.Default.LoadFromAssemblyPath(assembly.Location))
            .ToList()
            .AsReadOnly();

        return loadedAssemblies;
    }
}