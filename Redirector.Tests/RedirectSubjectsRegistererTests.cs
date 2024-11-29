using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Redirector.Tests;

public class RedirectSubjectsRegistererTests
{
    private readonly Mock<IAssemblyLoader> _assemblyLoaderMock = new();
    private readonly Mock<ILogger> _loggerMock = new();

    private readonly IOptions<PluginLoaderOptions> _options = Options.Create(new PluginLoaderOptions
        { ValueProvidersPath = "testPath" });

    [Fact]
    public void AddRedirectSubjects_ShouldLogError_WhenPathIsEmpty()
    {
        // Arrange
        var options = Options.Create(new PluginLoaderOptions());
        var registerer = new RedirectSubjectsRegisterer(_assemblyLoaderMock.Object, options, _loggerMock.Object);
        var services = new ServiceCollection();

        // Act
        registerer.AddRedirectSubjects(services);

        // Assert
        _loggerMock.Verify(
            logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("PluginLoader:ValueProvidersPath is empty")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }

    [Fact]
    public void AddRedirectSubjects_ShouldLogWarning_WhenNoAssembliesFound()
    {
        // Arrange
        _assemblyLoaderMock
            .Setup(loader => loader.Load("testPath", typeof(IRedirectSubject)))
            .Returns(new List<Assembly>());
        var registerer = new RedirectSubjectsRegisterer(_assemblyLoaderMock.Object, _options, _loggerMock.Object);
        var services = new ServiceCollection();

        // Act
        registerer.AddRedirectSubjects(services);

        // Assert
        _loggerMock.Verify(
            logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("No suitable assemblies found in 'testPath")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }

    [Fact]
    public void AddRedirectSubjects_ShouldRegisterServices_WhenValidTypesFound()
    {
        // Arrange
        var mockAssembly = new Mock<Assembly>();
        var mockType = typeof(TestSubject);

        mockAssembly
            .Setup(assembly => assembly.GetTypes())
            .Returns([mockType]);

        _assemblyLoaderMock
            .Setup(loader => loader.Load("testPath", typeof(IRedirectSubject)))
            .Returns(new List<Assembly> { mockAssembly.Object });

        var registerer = new RedirectSubjectsRegisterer(_assemblyLoaderMock.Object, _options, _loggerMock.Object);
        var services = new ServiceCollection();

        // Act
        registerer.AddRedirectSubjects(services);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var registeredService = serviceProvider.GetKeyedService<IRedirectSubject>("TestSubject");

        Assert.NotNull(registeredService);
        Assert.IsType<TestSubject>(registeredService);

        _loggerMock.Verify(
            logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) =>
                    o.ToString()!.Contains("Redirect subject 'TestSubject' was successfully registered.")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }

    [RedirectSubjectName("TestSubject")]
    private class TestSubject : SubjectValueProvider<int>
    {
        protected override int Value => 123;
    }
}