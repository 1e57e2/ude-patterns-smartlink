using System.Text.Json;

namespace Redirector;

public interface ISmartLinkEditorService
{
    Task<bool> CreateSmartLinkAsync(SmartLinkDescription smartLink, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SmartLinkDescription>> GetSmartLinks(CancellationToken cancellationToken = default);
    Task<SmartLinkDescription?> GetSmartLinks(string linkPath, CancellationToken cancellationToken = default);
    Task<bool> UpdateSmartLinkAsync(SmartLinkDescription smartLink, CancellationToken cancellationToken = default);
    Task<bool> DeleteSmartLinkAsync(string smartLink, CancellationToken cancellationToken = default);
}