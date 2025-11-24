using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.Exceptions;
using RecurPixel.EasyMessages.Formatters;
using Xunit;

namespace RecurPixel.EasyMessages.Tests.Unit;

public class FormatterRegistryTests : UnitTestBase
{
    [Fact]
    public void Register_ShouldAddFormatter()
    {
        Dispose();
        // Arrange
        var formatter = new TestFormatter();

        // Act
        FormatterRegistry.RegisterSingleton("test", formatter);

        // Assert
        Assert.True(FormatterRegistry.IsRegistered("test"));
    }

    [Fact]
    public void Get_ShouldReturnRegisteredFormatter()
    {
        Dispose();
        // Arrange
        var formatter = new TestFormatter();
        FormatterRegistry.RegisterSingleton("test2", formatter);

        // Act
        var retrieved = FormatterRegistry.Get("test2");

        // Assert
        Assert.Same(formatter, retrieved);
    }

    [Fact]
    public void Get_ShouldThrowForUnregisteredFormatter()
    {
        Dispose();
        // Act & Assert
        Assert.Throws<FormatterNotFoundException>(() => FormatterRegistry.Get("nonexistent"));
    }

    [Fact]
    public void GetRegisteredNames_ShouldReturnAllFormatters()
    {
        Dispose();
        // Act
        var names = FormatterRegistry.GetRegisteredNames().ToList();

        // Assert
        Assert.Contains("json", names);
        Assert.Contains("xml", names);
        Assert.Contains("text", names);
        Assert.Contains("console", names);
        Assert.Contains("log", names);
    }

    private class TestFormatter : IMessageFormatter
    {
        public string Format(Message message) => message.Title;

        public object FormatAsObject(Message message) => message;
    }
}
