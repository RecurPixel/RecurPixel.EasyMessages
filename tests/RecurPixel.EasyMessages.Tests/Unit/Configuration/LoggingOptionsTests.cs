using FluentAssertions;
using Microsoft.Extensions.Logging;
using RecurPixel.EasyMessages.AspNetCore.Configuration;

namespace RecurPixel.EasyMessages.Tests.Unit.Configuration;

public class LoggingOptionsTests
{
    [Fact]
    public void Constructor_ShouldHaveDefaultValues()
    {
        // Act
        var options = new LoggingOptions();

        // Assert
        options.AutoLog.Should().BeFalse();
        options.MinimumLogLevel.Should().Be(LogLevel.Warning);
    }

    [Theory]
    [InlineData(true, LogLevel.Debug)]
    [InlineData(true, LogLevel.Information)]
    [InlineData(true, LogLevel.Warning)]
    [InlineData(true, LogLevel.Error)]
    [InlineData(false, LogLevel.None)]
    public void Properties_ShouldBeSettable(bool autoLog, LogLevel logLevel)
    {
        // Arrange
        var options = new LoggingOptions();

        // Act
        options.AutoLog = autoLog;
        options.MinimumLogLevel = logLevel;

        // Assert
        options.AutoLog.Should().Be(autoLog);
        options.MinimumLogLevel.Should().Be(logLevel);
    }

    [Fact]
    public void AutoLog_WhenDisabled_MinimumLogLevelIsIrrelevant()
    {
        // Arrange
        var options = new LoggingOptions
        {
            AutoLog = false,
            MinimumLogLevel = LogLevel.Debug
        };

        // Assert
        // This is valid configuration - MinimumLogLevel is simply ignored when AutoLog is false
        options.AutoLog.Should().BeFalse();
        options.MinimumLogLevel.Should().Be(LogLevel.Debug);
    }
}
