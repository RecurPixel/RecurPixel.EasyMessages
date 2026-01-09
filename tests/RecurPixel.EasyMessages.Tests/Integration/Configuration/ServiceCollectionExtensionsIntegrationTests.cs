using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RecurPixel.EasyMessages.AspNetCore;
using RecurPixel.EasyMessages.AspNetCore.Configuration;

namespace RecurPixel.EasyMessages.Tests.Integration.Configuration;

public class ServiceCollectionExtensionsIntegrationTests
{
    [Fact]
    public void AddEasyMessages_WithConfiguration_ShouldRegisterServices()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["EasyMessages:Logging:AutoLog"] = "true",
                ["EasyMessages:Logging:MinimumLogLevel"] = "Information"
            })
            .Build();

        var services = new ServiceCollection();

        // Act
        services.AddEasyMessages(configuration);
        var provider = services.BuildServiceProvider();

        // Assert
        var options = provider.GetService<IOptions<EasyMessagesOptions>>();
        options.Should().NotBeNull();
        options!.Value.Should().NotBeNull();
    }

    [Fact]
    public void AddEasyMessages_WithConfiguration_ShouldBindFromAppSettings()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["EasyMessages:Logging:AutoLog"] = "true",
                ["EasyMessages:Logging:MinimumLogLevel"] = "Debug",
                ["EasyMessages:Formatter:IncludeTimestamp"] = "false",
                ["EasyMessages:Interceptor:AutoAddCorrelationId"] = "false",
                ["EasyMessages:Localization:DefaultLocale"] = "es-ES"
            })
            .Build();

        var services = new ServiceCollection();

        // Act
        services.AddEasyMessages(configuration);
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<EasyMessagesOptions>>().Value;

        // Assert
        options.Logging.AutoLog.Should().BeTrue();
        options.Logging.MinimumLogLevel.Should().Be(Microsoft.Extensions.Logging.LogLevel.Debug);
        options.Formatter.IncludeTimestamp.Should().BeFalse();
        options.Interceptor.AutoAddCorrelationId.Should().BeFalse();
        options.Localization.DefaultLocale.Should().Be("es-ES");
    }

    [Fact]
    public void AddEasyMessages_WithProgrammaticOverride_ShouldOverrideAppSettings()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["EasyMessages:Logging:AutoLog"] = "false"
            })
            .Build();

        var services = new ServiceCollection();

        // Act
        services.AddEasyMessages(configuration, options =>
        {
            options.Logging.AutoLog = true; // Override
        });
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<EasyMessagesOptions>>().Value;

        // Assert
        options.Logging.AutoLog.Should().BeTrue();
    }

    [Fact]
    public void AddEasyMessages_WithPreset_ShouldUsePresetConfiguration()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();

        // Act
        services.AddEasyMessages(configuration, EasyMessagesPresets.Production);
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<EasyMessagesOptions>>().Value;

        // Assert
        options.Logging.MinimumLogLevel.Should().Be(Microsoft.Extensions.Logging.LogLevel.Warning);
        options.Formatter.IncludeData.Should().BeFalse();
        options.Interceptor.AutoEnrichMetadata.Should().BeFalse();
    }

    [Fact]
    public void AddEasyMessages_WithEmptyConfiguration_ShouldUseDefaults()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();

        // Act
        services.AddEasyMessages(configuration);
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<EasyMessagesOptions>>().Value;

        // Assert
        options.Logging.AutoLog.Should().BeFalse();
        options.Logging.MinimumLogLevel.Should().Be(Microsoft.Extensions.Logging.LogLevel.Warning);
        options.Formatter.IncludeTimestamp.Should().BeTrue();
        options.Localization.DefaultLocale.Should().Be("en-US");
    }

    // Skipping this test - EasyMessagesConfigurator is internal
    // [Fact]
    // public void AddEasyMessages_ShouldRegisterConfigurator()
    // {
    //     // Arrange
    //     var configuration = new ConfigurationBuilder().Build();
    //     var services = new ServiceCollection();

    //     // Act
    //     services.AddEasyMessages(configuration);
    //     var provider = services.BuildServiceProvider();

    //     // Assert
    //     var configurator = provider.GetService<EasyMessagesConfigurator>();
    //     configurator.Should().NotBeNull();
    // }

    [Fact]
    public void AddEasyMessages_ShouldRegisterStartupService()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();

        // Act
        services.AddEasyMessages(configuration);

        // Assert
        var hasStartupService = services.Any(sd =>
            sd.ServiceType.Name == "IHostedService" &&
            sd.ImplementationType != null &&
            sd.ImplementationType.Name == "EasyMessagesStartupService");
        hasStartupService.Should().BeTrue();
    }

    [Fact]
    public void AddEasyMessages_WithComplexNestedConfiguration_ShouldBindCorrectly()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["EasyMessages:Interceptor:AutoEnrichMetadata"] = "true",
                ["EasyMessages:Interceptor:MetadataFields:IncludeRequestPath"] = "true",
                ["EasyMessages:Interceptor:MetadataFields:IncludeRequestMethod"] = "false",
                ["EasyMessages:Interceptor:MetadataFields:IncludeUserAgent"] = "true",
                ["EasyMessages:Interceptor:MetadataFields:IncludeIpAddress"] = "false",
                ["EasyMessages:Interceptor:MetadataFields:IncludeUserId"] = "true",
                ["EasyMessages:Interceptor:MetadataFields:IncludeUserName"] = "false"
            })
            .Build();

        var services = new ServiceCollection();

        // Act
        services.AddEasyMessages(configuration);
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<EasyMessagesOptions>>().Value;

        // Assert
        options.Interceptor.AutoEnrichMetadata.Should().BeTrue();
        options.Interceptor.MetadataFields.IncludeRequestPath.Should().BeTrue();
        options.Interceptor.MetadataFields.IncludeRequestMethod.Should().BeFalse();
        options.Interceptor.MetadataFields.IncludeUserAgent.Should().BeTrue();
        options.Interceptor.MetadataFields.IncludeIpAddress.Should().BeFalse();
        options.Interceptor.MetadataFields.IncludeUserId.Should().BeTrue();
        options.Interceptor.MetadataFields.IncludeUserName.Should().BeFalse();
    }

    [Fact]
    public void AddEasyMessages_WithStorageConfiguration_ShouldBindCorrectly()
    {
        // Arrange - Create temporary test files
        var tempCustom = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");
        var tempAuth = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");
        var tempPayment = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");
        File.WriteAllText(tempCustom, "{}");
        File.WriteAllText(tempAuth, "{}");
        File.WriteAllText(tempPayment, "{}");

        try
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["EasyMessages:Storage:CustomMessagesPath"] = tempCustom,
                    ["EasyMessages:Storage:CustomStorePaths:0"] = tempAuth,
                    ["EasyMessages:Storage:CustomStorePaths:1"] = tempPayment
                })
                .Build();

            var services = new ServiceCollection();

            // Act
            services.AddEasyMessages(configuration);
            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<EasyMessagesOptions>>().Value;

            // Assert
            options.Storage.CustomMessagesPath.Should().Be(tempCustom);
            options.Storage.CustomStorePaths.Should().NotBeNull();
            options.Storage.CustomStorePaths.Should().HaveCount(2);
            options.Storage.CustomStorePaths![0].Should().Be(tempAuth);
            options.Storage.CustomStorePaths[1].Should().Be(tempPayment);
        }
        finally
        {
            // Cleanup temp files
            if (File.Exists(tempCustom)) File.Delete(tempCustom);
            if (File.Exists(tempAuth)) File.Delete(tempAuth);
            if (File.Exists(tempPayment)) File.Delete(tempPayment);
        }
    }

    [Fact]
    public void AddEasyMessages_MultipleCallsWithDifferentPresets_LastOneWins()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();

        // Act
        services.AddEasyMessages(configuration, EasyMessagesPresets.Development);
        services.AddEasyMessages(configuration, EasyMessagesPresets.Production); // Should override

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<EasyMessagesOptions>>().Value;

        // Assert - Should use Production settings
        options.Logging.MinimumLogLevel.Should().Be(Microsoft.Extensions.Logging.LogLevel.Warning);
        options.Formatter.IncludeData.Should().BeFalse();
    }

    [Fact]
    public void AddEasyMessages_WithIOptionsMonitor_ShouldSupportHotReload()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["EasyMessages:Logging:AutoLog"] = "true"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddEasyMessages(configuration);
        var provider = services.BuildServiceProvider();

        // Act
        var monitor = provider.GetRequiredService<IOptionsMonitor<EasyMessagesOptions>>();

        // Assert
        monitor.Should().NotBeNull();
        monitor.CurrentValue.Should().NotBeNull();
        monitor.CurrentValue.Logging.AutoLog.Should().BeTrue();
    }

    [Fact]
    public void AddEasyMessages_WithIOptionsSnapshot_ShouldBeAvailable()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        services.AddEasyMessages(configuration);
        var provider = services.BuildServiceProvider();

        // Act
        using var scope = provider.CreateScope();
        var snapshot = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<EasyMessagesOptions>>();

        // Assert
        snapshot.Should().NotBeNull();
        snapshot.Value.Should().NotBeNull();
    }
}
