using System.Text.Json;
using System.Text.Json.Serialization;

namespace Redirector.Tests;

public class ObjectConverterTests
{
    private readonly ObjectConverter _converter = new();

    [Theory]
    [InlineData("\"test\"", "test")]
    [InlineData("123", 123)]
    [InlineData("1234567890123", 1234567890123L)]
    [InlineData("123.45", 123.45)]
    [InlineData("true", true)]
    [InlineData("false", false)]
    [InlineData("null", null)]
    public void Read_ShouldDeserializePrimitiveValues(string json, object? expected)
    {
        // Arrange
        var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(json));
        reader.Read();

        // Act
        var result = _converter.Read(ref reader, typeof(object), new JsonSerializerOptions());

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Read_ShouldDeserializeArray()
    {
        // Arrange
        var json = "[1, \"string\", true, null]";
        var options = new JsonSerializerOptions
        {
            Converters = { _converter }
        };

        // Act
        var result = JsonSerializer.Deserialize<object>(json, options);

        // Assert
        Assert.NotNull(result);
        var list = Assert.IsType<List<object>>(result);
        Assert.Equal(4, list.Count);
        Assert.Equal(1, list[0]);
        Assert.Equal("string", list[1]);
        Assert.Equal(true, list[2]);
        Assert.Null(list[3]);
    }

    [Fact]
    public void Read_ShouldDeserializeObject()
    {
        // Arrange
        var json = "{\"key1\": 123, \"key2\": \"value\", \"key3\": true}";
        var options = new JsonSerializerOptions
        {
            Converters = { _converter }
        };
        
        // Act
        var result = JsonSerializer.Deserialize<object>(json, options);

        // Assert
        Assert.NotNull(result);
        var dictionary = Assert.IsType<Dictionary<string, object>>(result);
        Assert.Equal(3, dictionary.Count);
        Assert.Equal(123, dictionary["key1"]);
        Assert.Equal("value", dictionary["key2"]);
        Assert.Equal(true, dictionary["key3"]);
    }

    [Fact]
    public void Write_ShouldSerializePrimitiveValues()
    {
        // Arrange
        var options = new JsonSerializerOptions { Converters = { _converter } };

        // Act & Assert
        Assert.Equal("\"string\"", JsonSerializer.Serialize("string", options));
        Assert.Equal("123", JsonSerializer.Serialize(123, options));
        Assert.Equal("1234567890123", JsonSerializer.Serialize(1234567890123L, options));
        Assert.Equal("123.45", JsonSerializer.Serialize(123.45, options));
        Assert.Equal("true", JsonSerializer.Serialize(true, options));
        Assert.Equal("false", JsonSerializer.Serialize(false, options));
        Assert.Equal("null", JsonSerializer.Serialize<object>(null!, options));
    }

    [Fact]
    public void Write_ShouldSerializeArray()
    {
        // Arrange
        var options = new JsonSerializerOptions { Converters = { _converter } };
        var array = new List<object?> { 1, "string", true, null };

        // Act
        var json = JsonSerializer.Serialize(array, options);

        // Assert
        Assert.Equal("[1,\"string\",true,null]", json);
    }

    [Fact]
    public void Write_ShouldSerializeObject()
    {
        // Arrange
        var options = new JsonSerializerOptions { Converters = { _converter } };
        var obj = new Dictionary<string, object>
        {
            { "key1", 123 },
            { "key2", "value" },
            { "key3", true }
        };

        // Act
        var json = JsonSerializer.Serialize(obj, options);

        // Assert
        Assert.Equal("{\"key1\":123,\"key2\":\"value\",\"key3\":true}", json);
    }
}
