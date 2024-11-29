using System.Text.Json;
using System.Text.RegularExpressions;

namespace Redirector.Tests;

public class SmartLinkDescriptionConverterTests
{
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        Converters = { new SmartLinkDescriptionConverter() }
    };

    [Fact]
    public void Read_ShouldDeserializeCorrectly()
    {
        // Arrange
        var json = @"{
            ""LinkPath"": ""/test"",
            ""State"": ""enabled"",
            ""Redirects"": [
                {
                    ""RedirectUrl"": ""https://example.org"",
                    ""Rules"": [
                        {
                            ""SubjectName"": ""Browser"",
                            ""Predicates"": {
                                ""EqualsTo"": ""Chrome""
                            }
                        }
                    ]
                }
            ]
        }";

        // Act
        var result = JsonSerializer.Deserialize<SmartLinkDescription>(json, _serializerOptions);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("/test", result.LinkPath);
        Assert.Equal("enabled", result.Description.GetProperty("State").GetString());
    }

    [Fact]
    public void Write_ShouldSerializeCorrectly()
    {
        // Arrange
        var smartLink = new SmartLinkDescription
        {
            LinkPath = "/test",
            Description = JsonDocument.Parse(@"{
                ""State"": ""enabled"",
                ""Redirects"": [
                    {
                        ""RedirectUrl"": ""https://example.org"",
                        ""Rules"": [
                            {
                                ""SubjectName"": ""Browser"",
                                ""Predicates"": {
                                    ""EqualsTo"": ""Chrome""
                                }
                            }
                        ]
                    }
                ]
            }").RootElement
        };

        // Act
        var json = JsonSerializer.Serialize(smartLink, _serializerOptions);

        // Assert
        var deserializedJson = JsonDocument.Parse(json).RootElement;
        Assert.Equal("/test", deserializedJson.GetProperty("LinkPath").GetString());
        Assert.Equal("enabled", deserializedJson.GetProperty("State").GetString());
    }

    [Fact]
    public void ReadAndWrite_ShouldBeSymmetric()
    {
        // Arrange
        var originalJson = @"{
            ""LinkPath"": ""/test"",
            ""State"": ""enabled"",
            ""Redirects"": [
                {
                    ""RedirectUrl"": ""https://example.org"",
                    ""Rules"": [
                        {
                            ""SubjectName"": ""Browser"",
                            ""Predicates"": {
                                ""EqualsTo"": ""Chrome""
                            }
                        }
                    ]
                }
            ]
        }";

        // Act
        var deserialized = JsonSerializer.Deserialize<SmartLinkDescription>(originalJson, _serializerOptions);
        var serializedJson = JsonSerializer.Serialize(deserialized, _serializerOptions);
        var reDeserialized = JsonSerializer.Deserialize<SmartLinkDescription>(serializedJson, _serializerOptions);
        
        var deserializedDescriptionText = Regex.Replace(deserialized?.Description.GetRawText() ?? string.Empty, @"\s+", "");
        var reDeserializedDescriptionText = Regex.Replace(reDeserialized?.Description.GetRawText() ?? string.Empty, @"\s+", "");

        // Assert
        Assert.NotNull(deserialized);
        Assert.NotNull(reDeserialized);
        Assert.Equal(deserialized.LinkPath, reDeserialized.LinkPath);
        Assert.Equal(deserializedDescriptionText, reDeserializedDescriptionText);
    }
}