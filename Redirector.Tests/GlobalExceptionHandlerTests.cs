using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Redirector.Tests;

public class GlobalExceptionHandlerTests
{
    [Fact]
    public async Task TryHandleAsync_ShouldLogErrorAndReturnProblemDetails()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<GlobalExceptionHandler>>();
        var handler = new GlobalExceptionHandler(loggerMock.Object);

        var context = new DefaultHttpContext();
        context.Request.Method = "GET";
        context.Request.Path = "/test-endpoint";
        context.Response.Body = new MemoryStream();

        var exception = new InvalidOperationException("Test exception");

        // Act
        var result = await handler.TryHandleAsync(context, exception, CancellationToken.None);

        // Assert
        // Проверяем, что исключение было залогировано
        loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("An unexpected error occurred")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);

        // Проверяем, что метод вернул true
        Assert.True(result);

        // Проверяем формат ответа
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseContent = await new StreamReader(context.Response.Body).ReadToEndAsync();

        var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(problemDetails);
        Assert.Equal(StatusCodes.Status500InternalServerError, problemDetails!.Status);
        Assert.Equal("InvalidOperationException", problemDetails.Type);
        Assert.Equal("Test exception", problemDetails.Detail);
        Assert.Equal("GET /test-endpoint", problemDetails.Instance);
    }
}
