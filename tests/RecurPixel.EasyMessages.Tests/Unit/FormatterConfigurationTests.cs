using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.Configuration;
using RecurPixel.EasyMessages.Core;
using RecurPixel.EasyMessages.Formatters;
using Xunit;

namespace RecurPixel.EasyMessages.Tests.Unit;

public class FormatterConfigurationTests : UnitTestBase
{
    [Fact]
    public void Configure_ShouldSetGlobalOptions()
    {
        Dispose();
        // Arrange & Act
        FormatterConfiguration.Configure(options =>
        {
            options.IncludeTimestamp = false;
            options.IncludeMetadata = false;
        });

        var message = MessageRegistry.Get("AUTH_001");
        var json = message.ToJson();

        // Assert
        Assert.DoesNotContain("timestamp", json);

        // Cleanup
        FormatterConfiguration.Reset();
    }

    [Fact]
    public void Minimal_ShouldIncludeOnlyEssentials()
    {
        Dispose();
        // Arrange
        var options = FormatterConfiguration.Minimal;

        // Assert
        Assert.False(options.IncludeTimestamp);
        Assert.False(options.IncludeCorrelationId);
        Assert.False(options.IncludeHttpStatusCode);
    }

    [Fact]
    public void Verbose_ShouldIncludeEverything()
    {
        Dispose();
        // Arrange
        var options = FormatterConfiguration.Verbose;

        // Assert
        Assert.True(options.IncludeTimestamp);
        Assert.True(options.IncludeCorrelationId);
        Assert.True(options.IncludeMetadata);
    }

    [Fact]
    public void Reset_ShouldRestoreDefaults()
    {
        Dispose();
        // Arrange
        FormatterConfiguration.Configure(options =>
        {
            options.IncludeTimestamp = false;
        });

        // Act
        FormatterConfiguration.Reset();

        // Assert
        var options = FormatterConfiguration.DefaultOptions;
        Assert.True(options.IncludeTimestamp); // Back to default
    }
}
