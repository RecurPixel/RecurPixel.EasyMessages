using FluentAssertions;
using RecurPixel.EasyMessages.AspNetCore.Configuration;

namespace RecurPixel.EasyMessages.Tests.Unit.Configuration;

public class LocalizationOptionsTests
{
    [Fact]
    public void Constructor_ShouldHaveDefaultValues()
    {
        // Act
        var options = new LocalizationOptions();

        // Assert
        options.DefaultLocale.Should().Be("en-US");
        options.EnableLocalization.Should().BeFalse();
        options.FallbackToDefault.Should().BeTrue();
    }

    [Theory]
    [InlineData("en-US")]
    [InlineData("es-ES")]
    [InlineData("fr-FR")]
    [InlineData("de-DE")]
    [InlineData("ja-JP")]
    [InlineData("zh-CN")]
    public void DefaultLocale_ShouldAcceptValidFormats(string locale)
    {
        // Arrange
        var options = new LocalizationOptions();

        // Act
        options.DefaultLocale = locale;

        // Assert
        options.DefaultLocale.Should().Be(locale);
    }

    [Fact]
    public void EnableLocalization_ShouldBeSettable()
    {
        // Arrange
        var options = new LocalizationOptions();

        // Act
        options.EnableLocalization = true;

        // Assert
        options.EnableLocalization.Should().BeTrue();
    }

    [Fact]
    public void FallbackToDefault_ShouldBeSettable()
    {
        // Arrange
        var options = new LocalizationOptions();

        // Act
        options.FallbackToDefault = false;

        // Assert
        options.FallbackToDefault.Should().BeFalse();
    }

    [Fact]
    public void Configuration_WithLocalizationDisabled_IsValid()
    {
        // Arrange
        var options = new LocalizationOptions
        {
            EnableLocalization = false,
            DefaultLocale = "en-US"
        };

        // Assert
        options.EnableLocalization.Should().BeFalse();
        options.DefaultLocale.Should().Be("en-US");
    }

    [Fact]
    public void Configuration_WithLocalizationEnabled_IsValid()
    {
        // Arrange
        var options = new LocalizationOptions
        {
            EnableLocalization = true,
            DefaultLocale = "fr-FR",
            FallbackToDefault = true
        };

        // Assert
        options.EnableLocalization.Should().BeTrue();
        options.DefaultLocale.Should().Be("fr-FR");
        options.FallbackToDefault.Should().BeTrue();
    }
}
