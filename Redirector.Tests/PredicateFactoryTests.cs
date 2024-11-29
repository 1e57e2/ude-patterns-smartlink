using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;

namespace Redirector.Tests;

public class PredicateFactoryTests
{
    private PredicateFactory CreateFactory(ServiceCollection services, Action<PredicateFactoryOptions>? configureOptions = null)
    {
        // Настройка конфигурации операций
        var options = Options.Create(new PredicateFactoryOptions());
        configureOptions?.Invoke(options.Value);

        // Регистрация зависимостей
        var serviceProvider = services.BuildServiceProvider();
        var mockKeyedServiceProvider = new Mock<IKeyedServiceProviderWrapper>();
        mockKeyedServiceProvider
            .Setup(provider => provider.CreateScope())
            .Returns(serviceProvider.CreateScope());

        return new PredicateFactory(mockKeyedServiceProvider.Object, options);
    }

    [Fact]
    public void Should_Create_EqualsBoolExpression_Predicate()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddKeyedScoped<IRedirectSubject, TestSubject>("TestSubject");

        var factory = CreateFactory(services, options =>
        {
            options.Operations.Add("EqualsTo", typeof(EqualsBoolExpression<>));
        });

        // Act
        var predicate = factory.CreatePredicate("TestSubject", "EqualsTo", "TestValue");

        // Assert
        Assert.NotNull(predicate);
        Assert.IsType<EqualsBoolExpression<string>>(predicate);
        Assert.True(predicate.Evaluate());
    }

    [Fact]
    public void Should_Create_IsInBoolExpression_Predicate()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddKeyedScoped<IRedirectSubject, TestSubject>("TestSubject");

        var factory = CreateFactory(services, options =>
        {
            options.Operations.Add("IsIn", typeof(IsInBoolExpression<>));
        });

        // Act
        var predicate = factory.CreatePredicate("TestSubject", "IsIn", new[] { "TestValue", "OtherValue" });

        // Assert
        Assert.NotNull(predicate);
        Assert.IsType<IsInBoolExpression<string>>(predicate);
        Assert.True(predicate.Evaluate());
    }

    [Fact]
    public void Should_Throw_Exception_For_Unknown_Operation()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddKeyedScoped<IRedirectSubject, TestSubject>("TestSubject");

        var factory = CreateFactory(services);

        // Act & Assert
        var exception = Assert.Throws<InvalidRedirectRuleException>(() =>
            factory.CreatePredicate("TestSubject", "UnknownOperation", "Value"));

        Assert.Equal("Unknown predicate operation 'UnknownOperation'.", exception.Message);
    }

    [Fact]
    public void Should_Throw_Exception_If_Subject_Not_Found()
    {
        // Arrange
        var services = new ServiceCollection();

        var factory = CreateFactory(services);

        // Act & Assert
        var exception = Assert.Throws<InvalidRedirectRuleException>(() =>
            factory.CreatePredicate("NonExistentSubject", "EqualsTo", "Value"));

        Assert.Equal("Value provider for NonExistentSubject not found", exception.Message);
    }

    [Fact]
    public void Should_Throw_Exception_For_Invalid_Value_Format()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddKeyedScoped<IRedirectSubject, TestSubjectWithInt>("TestSubject");

        var factory = CreateFactory(services, options =>
        {
            options.Operations.Add("EqualsTo", typeof(EqualsBoolExpression<>));
        });

        // Act & Assert
        var exception = Assert.Throws<InvalidRedirectRuleException>(() =>
            factory.CreatePredicate("TestSubject", "EqualsTo", "InvalidNumber"));

        Assert.Equal("Cannot parse 'InvalidNumber' to type 'Int32'.", exception.Message);
    }

    private class TestSubject : SubjectValueProvider<string>
    {
        protected override string Value => "TestValue";
    }

    private class TestSubjectWithInt : SubjectValueProvider<int>
    {
        protected override int Value => 123;
    }
}