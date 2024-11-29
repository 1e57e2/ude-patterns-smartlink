using System.Reflection;

namespace Redirector;

public interface IAssemblyLoader
{
    IReadOnlyCollection<Assembly> Load(string directoryPath, Type type);
}