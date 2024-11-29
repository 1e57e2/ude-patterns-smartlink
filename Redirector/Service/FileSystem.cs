namespace Redirector;

public class FileSystem : IFileSystem
{
    public IEnumerable<string> GetFiles(string path, string searchPattern)
    {
        return Directory.GetFiles(path, searchPattern);
    }
}