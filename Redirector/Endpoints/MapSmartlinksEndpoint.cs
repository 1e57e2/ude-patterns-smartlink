using System.Text.Json;

namespace Redirector;

public static class SmartlinkEndpoint
{
    public static void MapSmartlinksEditorEndpoint(this IEndpointRouteBuilder app)
    {
        var smartLinksGroup = app.MapGroup("/api/smartlinks");

        // Create
        smartLinksGroup.MapPost("/", async (JsonElement newLink,
            ISmartLinkEditorService smartLinks,
            ISmartLinkDescrptionValidator smartLinkValidator) =>
        {
            if(!smartLinkValidator.TryGetDescription(newLink, out var newLinkDescription, out var message))
                return Results.BadRequest(message);
            var result = await smartLinks.CreateSmartLinkAsync(newLinkDescription);
            return result ? Results.Created() : Results.Conflict($"Link with path '{newLinkDescription.LinkPath}' already exists.");
        });

        // Read all
        smartLinksGroup.MapGet("/", async (ISmartLinkEditorService smartLinks) =>
        {
            var links = await smartLinks.GetSmartLinks();
            var result = links.Select(l => l.Description).ToList();
            return Results.Ok(result);
        });

        // Read one
        smartLinksGroup.MapGet("/{linkPath}", async (string linkPath, ISmartLinkEditorService smartLinks) =>
        {
            linkPath = Uri.UnescapeDataString(linkPath);
            var smartLink = await smartLinks.GetSmartLinks(linkPath);
            return smartLink is not null
                ? Results.Ok(smartLink.Description)
                : Results.NotFound($"Link with path '{linkPath}' not found.");
        });
        
        // Update
        smartLinksGroup.MapPut("/", async (JsonElement updatedLink,
            ISmartLinkEditorService smartLinks,
            ISmartLinkDescrptionValidator smartLinkValidator) =>
        {
            if(!smartLinkValidator.TryGetDescription(updatedLink, out var updatedLinkDescription, out var message))
                return Results.BadRequest(message);
            if (!await smartLinks.UpdateSmartLinkAsync(updatedLinkDescription))
                return Results.NotFound($"Link with path '{updatedLinkDescription.LinkPath}' not found.");
            return Results.Ok();
        });
        
        // Delete
        smartLinksGroup.MapDelete("/{linkPath}", async (string linkPath, ISmartLinkEditorService smartLinks) =>
        {
            linkPath = Uri.UnescapeDataString(linkPath);
            if (!await smartLinks.DeleteSmartLinkAsync(linkPath))
                return Results.NotFound($"Link with path '{linkPath}' not found.");
            return Results.Ok();
        });
    }
}