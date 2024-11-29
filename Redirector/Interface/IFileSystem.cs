namespace Redirector;

public interface IFileSystem
{
    IEnumerable<string> GetFiles(string path, string searchPattern);
}