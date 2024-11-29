using System.Text.Json;

namespace Redirector;

public class SmartLinkDescription
{
    public required string LinkPath { get; set; }
    public required JsonElement Description { get; set; }
}