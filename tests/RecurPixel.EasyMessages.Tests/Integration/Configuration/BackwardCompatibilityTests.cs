using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RecurPixel.EasyMessages.AspNetCore;
using RecurPixel.EasyMessages.AspNetCore.Configuration;

namespace RecurPixel.EasyMessages.Tests.Integration.Configuration;

/// <summary>
/// Tests to ensure backward compatibility with legacy MessageConfiguration pattern
/// </summary>
public class BackwardCompatibilityTests
{
    [Fact]
    public void LegacyPattern_ShouldStillWork()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act - Using old pattern (marked obsolete but should still work)
        #pragma warning disable CS0618 // Type or member is obsolete
        services.AddEasyMessages(options =>
        {
            options.AutoLog = true;
            options.MinimumLogLevel = LogLevel.Information;
        });
        #pragma warning restore CS0618

        var provider = services.BuildServiceProvider();

        // Assert
        var config = provider.GetService<MessageConfiguration>();
        config.Should().NotBeNull();
        config!.AutoLog.Should().BeTrue();
        config.MinimumLogLevel.Should().Be(LogLevel.Information);
    }

    [Fact]
    public void LegacyPattern_WithFormatterOptions_ShouldWork()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        #pragma warning disable CS0618
        services.AddEasyMessages(options =>
        {
            options.FormatterOptions.IncludeTimestamp = false;
            options.FormatterOptions.IncludeCorrelationId = true;
            options.FormatterOptions.IncludeMetadata = false;
        });
        #pragma warning restore CS0618

        var provider = services.BuildServiceProvider();

        // Assert
        var config = provider.GetRequiredService<MessageConfiguration>();
        config.FormatterOptions.IncludeTimestamp.Should().BeFalse();
        config.FormatterOptions.IncludeCorrelationId.Should().BeTrue();
        config.FormatterOptions.IncludeMetadata.Should().BeFalse();
    }

    [Fact]
    public void LegacyPattern_WithInterceptorOptions_ShouldWork()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        #pragma warning disable CS0618
        services.AddEasyMessages(options =>
        {
            options.InterceptorOptions.AutoAddCorrelationId = false;
            options.InterceptorOptions.AutoEnrichMetadata = true;
        });
        #pragma warning restore CS0618

        var provider = services.BuildServiceProvider();

        // Assert
        var config = provider.GetRequiredService<MessageConfiguration>();
        config.InterceptorOptions.AutoAddCorrelationId.Should().BeFalse();
        config.InterceptorOptions.AutoEnrichMetadata.Should().BeTrue();
    }

    [Fact]
    public void ToEasyMessagesOptions_ShouldConvertCorrectly()
    {
        // Arrange
        #pragma warning disable CS0618
        var legacy = new MessageConfiguration
        {
            AutoLog = true,
            MinimumLogLevel = LogLevel.Debug,
            CustomMessagesPath = "custom.json",
            DefaultLocale = "fr-FR"
        };
        legacy.FormatterOptions.IncludeTimestamp = false;
        legacy.InterceptorOptions.AutoAddCorrelationId = false;

        // Act
        var newOptions = legacy.ToEasyMessagesOptions();
        #pragma warning restore CS0618

        // Assert
        newOptions.Logging.AutoLog.Should().BeTrue();
        newOptions.Logging.MinimumLogLevel.Should().Be(LogLevel.Debug);
        newOptions.Storage.CustomMessagesPath.Should().Be("custom.json");
        newOptions.Localization.DefaultLocale.Should().Be("fr-FR");
        newOptions.Formatter.IncludeTimestamp.Should().BeFalse();
        newOptions.Interceptor.AutoAddCorrelationId.Should().BeFalse();
    }

    [Fact]
    public void ToEasyMessagesOptions_ShouldCreateNewInstances()
    {
        // Arrange
        #pragma warning disable CS0618
        var legacy = new MessageConfiguration();

        // Act
        var newOptions = legacy.ToEasyMessagesOptions();
        #pragma warning restore CS0618

        // Modify new options
        newOptions.Logging.AutoLog = true;

        // Assert - Legacy should not be affected
        legacy.AutoLog.Should().BeFalse();
    }

    [Fact]
    public void ToEasyMessagesOptions_WithNullValues_ShouldHandleGracefully()
    {
        // Arrange
        #pragma warning disable CS0618
        var legacy = new MessageConfiguration
        {
            CustomMessagesPath = null,
            CustomStores = null,
            CustomFormatters = null,
            Interceptors = null
        };

        // Act
        var newOptions = legacy.ToEasyMessagesOptions();
        #pragma warning restore CS0618

        // Assert
        newOptions.Storage.CustomMessagesPath.Should().BeNull();
        newOptions.Storage.CustomStorePaths.Should().BeNull();
        newOptions.CustomStores.Should().BeNull();
        newOptions.CustomFormatters.Should().BeNull();
        newOptions.Interceptors.Should().BeNull();
    }

    [Fact]
    public void ToEasyMessagesOptions_ShouldSetLocalizationDefaults()
    {
        // Arrange
        #pragma warning disable CS0618
        var legacy = new MessageConfiguration
        {
            DefaultLocale = "es-ES"
        };

        // Act
        var newOptions = legacy.ToEasyMessagesOptions();
        #pragma warning restore CS0618

        // Assert
        newOptions.Localization.DefaultLocale.Should().Be("es-ES");
        newOptions.Localization.EnableLocalization.Should().BeFalse();
        newOptions.Localization.FallbackToDefault.Should().BeTrue();
    }

    [Fact]
    public void LegacyPattern_WithNullConfiguration_ShouldUseDefaults()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        #pragma warning disable CS0618
        services.AddEasyMessages(configure: null);
        #pragma warning restore CS0618

        var provider = services.BuildServiceProvider();

        // Assert
        var config = provider.GetRequiredService<MessageConfiguration>();
        config.AutoLog.Should().BeFalse();
        config.MinimumLogLevel.Should().Be(LogLevel.Warning);
        config.DefaultLocale.Should().Be("en-US");
    }

    [Fact]
    public void LegacyPattern_PropertyMappingValidation()
    {
        // This test documents the property mapping between old and new patterns

        #pragma warning disable CS0618
        var legacy = new MessageConfiguration
        {
            // Direct properties
            CustomMessagesPath = "test.json",
            CustomStores = new List<RecurPixel.EasyMessages.Storage.IMessageStore>(),
            CustomFormatters = new Dictionary<string, Func<RecurPixel.EasyMessages.Formatters.IMessageFormatter>>(),
            Interceptors = new List<RecurPixel.EasyMessages.Interceptors.IMessageInterceptor>(),
            AutoLog = true,
            MinimumLogLevel = LogLevel.Error,
            DefaultLocale = "de-DE"
        };

        var newOptions = legacy.ToEasyMessagesOptions();
        #pragma warning restore CS0618

        // Verify all mappings
        var mappings = new Dictionary<string, (object Expected, object Actual)>
        {
            ["CustomMessagesPath"] = (legacy.CustomMessagesPath, newOptions.Storage.CustomMessagesPath),
            ["CustomStores"] = (legacy.CustomStores, newOptions.CustomStores),
            ["CustomFormatters"] = (legacy.CustomFormatters, newOptions.CustomFormatters),
            ["Interceptors"] = (legacy.Interceptors, newOptions.Interceptors),
            ["AutoLog"] = (legacy.AutoLog, newOptions.Logging.AutoLog),
            ["MinimumLogLevel"] = (legacy.MinimumLogLevel, newOptions.Logging.MinimumLogLevel),
            ["DefaultLocale"] = (legacy.DefaultLocale, newOptions.Localization.DefaultLocale),
            ["FormatterOptions"] = (legacy.FormatterOptions, newOptions.Formatter),
            ["InterceptorOptions"] = (legacy.InterceptorOptions, newOptions.Interceptor)
        };

        foreach (var (property, (expected, actual)) in mappings)
        {
            actual.Should().Be(expected, $"Property {property} should map correctly");
        }
    }

    [Fact]
    public void ObsoleteAttribute_ShouldBePresent()
    {
        // Assert
        var messageConfigType = typeof(MessageConfiguration);
        var obsoleteAttr = messageConfigType.GetCustomAttributes(typeof(ObsoleteAttribute), false)
            .Cast<ObsoleteAttribute>()
            .FirstOrDefault();

        obsoleteAttr.Should().NotBeNull("MessageConfiguration should have ObsoleteAttribute");
        obsoleteAttr!.Message.Should().Contain("EasyMessagesOptions");
        obsoleteAttr.Message.Should().Contain("IOptions pattern");
    }

    [Fact]
    public void LegacyAddEasyMessages_ShouldHaveObsoleteAttribute()
    {
        // Assert
        var method = typeof(ServiceCollectionExtensions)
            .GetMethods()
            .Where(m => m.Name == "AddEasyMessages")
            .Where(m => m.GetParameters().Length == 2) // Legacy overload
            .Where(m => m.GetParameters()[1].ParameterType == typeof(Action<MessageConfiguration>))
            .FirstOrDefault();

        method.Should().NotBeNull("Legacy AddEasyMessages method should exist");

        var obsoleteAttr = method!.GetCustomAttributes(typeof(ObsoleteAttribute), false)
            .Cast<ObsoleteAttribute>()
            .FirstOrDefault();

        obsoleteAttr.Should().NotBeNull("Legacy AddEasyMessages should have ObsoleteAttribute");
        obsoleteAttr!.Message.Should().Contain("IConfiguration");
    }
}
