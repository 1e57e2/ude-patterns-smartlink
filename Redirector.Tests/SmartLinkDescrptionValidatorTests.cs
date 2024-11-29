// using System.Text.Json;
// using Xunit;
//
// namespace Redirector.Tests;
//
// public class SmartLinkDescrptionValidatorTests
// {
//     private readonly ISmartLinkDescrptionValidator _validator;
//     private readonly JsonSerializerOptions _serializerOptions;
//
//     public SmartLinkDescrptionValidatorTests()
//     {
//         _serializerOptions = new JsonSerializerOptions
//         {
//             PropertyNameCaseInsensitive = true // Игнорируем регистр имён свойств
//         };
//
//         _validator = new SmartLinkDescrptionValidator(_serializerOptions);
//     }
//
//     [Fact]
//     public void TryGetDescription_ShouldReturnTrue_WhenDataIsValid()
//     {
//         // Arrange
//         var validJson = JsonDocument.Parse(@"{
//             ""LinkPath"": ""/test"",
//             ""State"": ""enabled"",
//             ""Redirects"": [
//                 {
//                     ""RedirectUrl"": ""https://example.org"",
//                     ""Rules"": []
//                 }
//             ]
//         }").RootElement;
//
//         // Act
//         var result = _validator.TryGetDescription(validJson, out var linkDescription, out var message);
//
//         // Assert
//         Assert.True(result);
//         Assert.Null(message);
//         Assert.NotNull(linkDescription);
//         Assert.Equal("/test", linkDescription.LinkPath);
//     }
//
//     [Fact]
//     public void TryGetDescription_ShouldReturnFalse_WhenLinkPathIsMissing()
//     {
//         // Arrange
//         var invalidJson = JsonDocument.Parse(@"{
//             ""State"": ""enabled"",
//             ""Redirects"": [
//                 {
//                     ""RedirectUrl"": ""https://example.org"",
//                     ""Rules"": []
//                 }
//             ]
//         }").RootElement;
//
//         // Act
//         var result = _validator.TryGetDescription(invalidJson, out var linkDescription, out var message);
//
//         // Assert
//         Assert.False(result);
//         Assert.NotNull(message);
//         Assert.Contains("LinkPath", message, StringComparison.OrdinalIgnoreCase);
//         Assert.Null(linkDescription);
//     }
//
//     [Fact]
//     public void TryGetDescription_ShouldReturnFalse_WhenDescriptionIsInvalid()
//     {
//         // Arrange
//         var invalidJson = JsonDocument.Parse(@"{
//             ""LinkPath"": ""/test"",
//             ""State"": ""enabled"",
//             ""Redirects"": ""NotAnArray""
//         }").RootElement;
//
//         // Act
//         var result = _validator.TryGetDescription(invalidJson, out var linkDescription, out var message);
//
//         // Assert
//         Assert.False(result);
//         Assert.NotNull(message);
//         Assert.Contains("Redirects", message, StringComparison.OrdinalIgnoreCase);
//         Assert.Null(linkDescription);
//     }
//
//     [Fact]
//     public void TryGetDescription_ShouldReturnFalse_WhenJsonIsInvalid()
//     {
//         // Arrange
//         string invalidJsonString = "{ \"Invalid\": ";
//
//         JsonElement invalidJson;
//         try
//         {
//             invalidJson = JsonDocument.Parse(invalidJsonString).RootElement;
//         }
//         catch (JsonException)
//         {
//             invalidJson = default; // Симулируем ситуацию с невалидным JSON
//         }
//
//         // Act
//         var result = _validator.TryGetDescription(invalidJson, out var linkDescription, out var message);
//
//         // Assert
//         Assert.False(result);
//         Assert.NotNull(message);
//         Assert.Null(linkDescription);
//     }
// }
