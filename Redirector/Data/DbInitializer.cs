using System.Text.Json;

namespace Redirector;

public static class DbInitializer
{
    public static void Seed(RedirectsDbContext context, string sourceFilePath)
    {
        if (!context.SmartLinkDescription.Any())
        {
            var json = File.ReadAllText(sourceFilePath);
            var links = new List<SmartLinkDescription>();
            using var document = JsonDocument.Parse(json);
            foreach (var element in document.RootElement.EnumerateArray())
            {
                var linkPath = element.GetProperty("LinkPath").GetString();
                var description = element.Clone();

                links.Add(new SmartLinkDescription
                {
                    LinkPath = linkPath!,
                    Description = description
                });
            }
            context.SmartLinkDescription.AddRange(links);
        }
        context.SaveChanges();
    }
}