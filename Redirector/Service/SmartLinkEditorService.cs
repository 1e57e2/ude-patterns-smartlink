using Microsoft.EntityFrameworkCore;

namespace Redirector;

public class SmartLinkEditorService(RedirectsDbContext context) : ISmartLinkEditorService
{
    public async Task<bool> CreateSmartLinkAsync(SmartLinkDescription smartLink, CancellationToken cancellationToken)
    {
        if (context.SmartLinkDescription.Any(r => r.LinkPath == smartLink.LinkPath)) return false;
        context.SmartLinkDescription.Add(smartLink);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<IReadOnlyCollection<SmartLinkDescription>> GetSmartLinks(CancellationToken cancellationToken)
    {
        var links = await context.SmartLinkDescription
            .ToArrayAsync(cancellationToken);
        return links;
    }

    public async Task<SmartLinkDescription?> GetSmartLinks(string linkPath, CancellationToken cancellationToken)
    {
        var link = await context.SmartLinkDescription
            .FirstOrDefaultAsync(x => x.LinkPath == linkPath, cancellationToken);
        return link;
    }

    public async Task<bool> UpdateSmartLinkAsync(SmartLinkDescription smartLink, CancellationToken cancellationToken)
    {
        var existingLink = await context.SmartLinkDescription
            .FirstOrDefaultAsync(s => s.LinkPath == smartLink.LinkPath, cancellationToken);

        if (existingLink is null)
            return false;

        existingLink.Description = smartLink.Description;

        return await context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> DeleteSmartLinkAsync(string smartLink, CancellationToken cancellationToken)
    {
        var link = await context.SmartLinkDescription
            .FirstOrDefaultAsync(r => r.LinkPath == smartLink, cancellationToken);

        if (link is null)
            return false;

        context.SmartLinkDescription.Remove(link);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }
}