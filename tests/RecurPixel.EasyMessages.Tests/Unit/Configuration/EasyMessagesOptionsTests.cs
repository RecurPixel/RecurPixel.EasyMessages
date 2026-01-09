using FluentAssertions;
using Microsoft.Extensions.Logging;
using RecurPixel.EasyMessages.AspNetCore.Configuration;
using RecurPixel.EasyMessages.Formatters;

namespace RecurPixel.EasyMessages.Tests.Unit.Configuration;

public class EasyMessagesOptionsTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Act
        var options = new EasyMessagesOptions();

        // Assert
        options.Formatter.Should().NotBeNull();
        options.Interceptor.Should().NotBeNull();
        options.Logging.Should().NotBeNull();
        options.Storage.Should().NotBeNull();
        options.Localization.Should().NotBeNull();

        // Verify nested defaults
        options.Formatter.IncludeTimestamp.Should().BeTrue();
        options.Interceptor.AutoAddCorrelationId.Should().BeTrue();
        options.Logging.AutoLog.Should().BeFalse();
        options.Logging.MinimumLogLevel.Should().Be(LogLevel.Warning);
        options.Localization.DefaultLocale.Should().Be("en-US");
    }

    [Fact]
    public void Clone_ShouldCreateDeepCopy()
    {
        // Arrange
        var original = new EasyMessagesOptions
        {
            Formatter = new FormatterOptions { IncludeTimestamp = false },
            Logging = new LoggingOptions { AutoLog = true, MinimumLogLevel = LogLevel.Debug },
            Storage = new StorageOptions { CustomMessagesPath = "test.json" },
            Localization = new LocalizationOptions { DefaultLocale = "es-ES" }
        };

        // Act
        var clone = original.Clone();

        // Assert
        clone.Should().NotBeSameAs(original);
        clone.Formatter.Should().NotBeSameAs(original.Formatter);
        clone.Logging.Should().NotBeSameAs(original.Logging);
        clone.Storage.Should().NotBeSameAs(original.Storage);
        clone.Localization.Should().NotBeSameAs(original.Localization);

        // Verify values were copied
        clone.Formatter.IncludeTimestamp.Should().Be(original.Formatter.IncludeTimestamp);
        clone.Logging.AutoLog.Should().Be(original.Logging.AutoLog);
        clone.Storage.CustomMessagesPath.Should().Be(original.Storage.CustomMessagesPath);
        clone.Localization.DefaultLocale.Should().Be(original.Localization.DefaultLocale);
    }

    [Fact]
    public void Clone_ModifyingClone_ShouldNotAffectOriginal()
    {
        // Arrange
        var original = new EasyMessagesOptions();
        var clone = original.Clone();

        // Act
        clone.Logging.AutoLog = true;
        clone.Formatter.IncludeTimestamp = false;

        // Assert
        original.Logging.AutoLog.Should().BeFalse();
        original.Formatter.IncludeTimestamp.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithValidConfiguration_ShouldReturnNoErrors()
    {
        // Arrange
        var options = new EasyMessagesOptions();
        var context = new System.ComponentModel.DataAnnotations.ValidationContext(options);

        // Act
        var results = options.Validate(context);

        // Assert
        results.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WithInvalidLocaleFormat_ShouldReturnError()
    {
        // Arrange
        var options = new EasyMessagesOptions
        {
            Localization = new LocalizationOptions { DefaultLocale = "invalid" }
        };
        var context = new System.ComponentModel.DataAnnotations.ValidationContext(options);

        // Act
        var results = options.Validate(context).ToList();

        // Assert
        results.Should().HaveCount(1);
        results[0].ErrorMessage.Should().Contain("Invalid locale format");
        results[0].ErrorMessage.Should().Contain("invalid");
    }

    // Note: File existence validation tests removed
    // File validation is now handled by EasyMessagesOptionsValidator (not IValidatableObject)
    // See EasyMessagesOptionsValidatorTests for comprehensive file validation testing

    [Theory]
    [InlineData("en-US", true)]
    [InlineData("es-ES", true)]
    [InlineData("fr-FR", true)]
    [InlineData("de-DE", true)]
    [InlineData("ja-JP", true)]
    [InlineData("en", false)]
    [InlineData("EN-US", false)]
    [InlineData("en-us", false)]
    [InlineData("invalid", false)]
    [InlineData("", false)]
    public void Validate_LocaleFormat_ShouldValidateCorrectly(string locale, bool shouldBeValid)
    {
        // Arrange
        var options = new EasyMessagesOptions
        {
            Localization = new LocalizationOptions { DefaultLocale = locale }
        };
        var context = new System.ComponentModel.DataAnnotations.ValidationContext(options);

        // Act
        var results = options.Validate(context).ToList();

        // Assert
        if (shouldBeValid)
        {
            results.Should().BeEmpty();
        }
        else
        {
            results.Should().HaveCountGreaterThan(0);
        }
    }

    [Fact]
    public void SectionName_ShouldBeEasyMessages()
    {
        // Assert
        EasyMessagesOptions.SectionName.Should().Be("EasyMessages");
    }
}
