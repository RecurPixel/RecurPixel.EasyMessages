using FluentAssertions;
using Microsoft.Extensions.Logging;
using RecurPixel.EasyMessages.AspNetCore.Configuration;

namespace RecurPixel.EasyMessages.Tests.Unit.Configuration;

public class EasyMessagesPresetsTests
{
    [Fact]
    public void Development_ShouldHaveVerboseConfiguration()
    {
        // Act
        var preset = EasyMessagesPresets.Development;

        // Assert
        preset.Should().NotBeNull();

        // Formatter should include everything
        preset.Formatter.IncludeTimestamp.Should().BeTrue();
        preset.Formatter.IncludeCorrelationId.Should().BeTrue();
        preset.Formatter.IncludeMetadata.Should().BeTrue();
        preset.Formatter.IncludeData.Should().BeTrue();
        preset.Formatter.IncludeParameters.Should().BeTrue();
        preset.Formatter.IncludeNullFields.Should().BeTrue();

        // Logging should be verbose
        preset.Logging.AutoLog.Should().BeTrue();
        preset.Logging.MinimumLogLevel.Should().Be(LogLevel.Debug);

        // Interceptors should be fully enabled
        preset.Interceptor.AutoAddCorrelationId.Should().BeTrue();
        preset.Interceptor.AutoEnrichMetadata.Should().BeTrue();

        // All metadata fields should be included
        preset.Interceptor.MetadataFields.IncludeRequestPath.Should().BeTrue();
        preset.Interceptor.MetadataFields.IncludeRequestMethod.Should().BeTrue();
        preset.Interceptor.MetadataFields.IncludeUserAgent.Should().BeTrue();
        preset.Interceptor.MetadataFields.IncludeIpAddress.Should().BeTrue();
        preset.Interceptor.MetadataFields.IncludeUserId.Should().BeTrue();
        preset.Interceptor.MetadataFields.IncludeUserName.Should().BeTrue();
    }

    [Fact]
    public void Production_ShouldHavePrivacySafeConfiguration()
    {
        // Act
        var preset = EasyMessagesPresets.Production;

        // Assert
        preset.Should().NotBeNull();

        // Formatter should exclude sensitive data
        preset.Formatter.IncludeTimestamp.Should().BeTrue();
        preset.Formatter.IncludeCorrelationId.Should().BeTrue();
        preset.Formatter.IncludeMetadata.Should().BeFalse(); // No metadata in production
        preset.Formatter.IncludeData.Should().BeFalse(); // No data in production
        preset.Formatter.IncludeParameters.Should().BeFalse();

        // Logging should capture warnings and errors
        preset.Logging.AutoLog.Should().BeTrue();
        preset.Logging.MinimumLogLevel.Should().Be(LogLevel.Warning);

        // Interceptors - correlation only, no metadata enrichment
        preset.Interceptor.AutoAddCorrelationId.Should().BeTrue();
        preset.Interceptor.AutoEnrichMetadata.Should().BeFalse();

        // No metadata fields should be included
        preset.Interceptor.MetadataFields.IncludeRequestPath.Should().BeFalse();
        preset.Interceptor.MetadataFields.IncludeRequestMethod.Should().BeFalse();
        preset.Interceptor.MetadataFields.IncludeUserAgent.Should().BeFalse();
        preset.Interceptor.MetadataFields.IncludeIpAddress.Should().BeFalse();
        preset.Interceptor.MetadataFields.IncludeUserId.Should().BeFalse();
        preset.Interceptor.MetadataFields.IncludeUserName.Should().BeFalse();
    }

    [Fact]
    public void Testing_ShouldHaveMinimalConfiguration()
    {
        // Act
        var preset = EasyMessagesPresets.Testing;

        // Assert
        preset.Should().NotBeNull();

        // Formatter should be minimal
        preset.Formatter.IncludeTimestamp.Should().BeFalse();
        preset.Formatter.IncludeCorrelationId.Should().BeFalse();
        preset.Formatter.IncludeMetadata.Should().BeFalse();
        preset.Formatter.IncludeData.Should().BeTrue(); // Keep data for assertions

        // No logging in tests
        preset.Logging.AutoLog.Should().BeFalse();
        preset.Logging.MinimumLogLevel.Should().Be(LogLevel.None);

        // No interceptors in tests
        preset.Interceptor.AutoAddCorrelationId.Should().BeFalse();
        preset.Interceptor.AutoEnrichMetadata.Should().BeFalse();
    }

    [Fact]
    public void Staging_ShouldHaveBalancedConfiguration()
    {
        // Act
        var preset = EasyMessagesPresets.Staging;

        // Assert
        preset.Should().NotBeNull();

        // Formatter should be verbose
        preset.Formatter.IncludeTimestamp.Should().BeTrue();
        preset.Formatter.IncludeCorrelationId.Should().BeTrue();
        preset.Formatter.IncludeMetadata.Should().BeTrue();
        preset.Formatter.IncludeData.Should().BeTrue();

        // Logging should be informational
        preset.Logging.AutoLog.Should().BeTrue();
        preset.Logging.MinimumLogLevel.Should().Be(LogLevel.Information);

        // Interceptors enabled with basic metadata
        preset.Interceptor.AutoAddCorrelationId.Should().BeTrue();
        preset.Interceptor.AutoEnrichMetadata.Should().BeTrue();

        // Basic metadata only (no PII)
        preset.Interceptor.MetadataFields.IncludeRequestPath.Should().BeTrue();
        preset.Interceptor.MetadataFields.IncludeRequestMethod.Should().BeTrue();
        preset.Interceptor.MetadataFields.IncludeUserAgent.Should().BeFalse();
        preset.Interceptor.MetadataFields.IncludeIpAddress.Should().BeFalse();
        preset.Interceptor.MetadataFields.IncludeUserId.Should().BeFalse();
        preset.Interceptor.MetadataFields.IncludeUserName.Should().BeFalse();
    }

    [Fact]
    public void Api_ShouldHaveCleanResponseConfiguration()
    {
        // Act
        var preset = EasyMessagesPresets.Api;

        // Assert
        preset.Should().NotBeNull();

        // Formatter should be clean for API consumers
        preset.Formatter.IncludeTimestamp.Should().BeFalse();
        preset.Formatter.IncludeCorrelationId.Should().BeFalse();
        preset.Formatter.IncludeHttpStatusCode.Should().BeTrue(); // Clients need this
        preset.Formatter.IncludeMetadata.Should().BeFalse();
        preset.Formatter.IncludeData.Should().BeTrue(); // Clients need data
        preset.Formatter.IncludeHint.Should().BeTrue(); // Help text for users

        // No automatic logging (log separately if needed)
        preset.Logging.AutoLog.Should().BeFalse();

        // Correlation ID for support, but no metadata
        preset.Interceptor.AutoAddCorrelationId.Should().BeTrue();
        preset.Interceptor.AutoEnrichMetadata.Should().BeFalse();
    }

    [Theory]
    [InlineData("Development")]
    [InlineData("development")]
    [InlineData("DEVELOPMENT")]
    public void ForEnvironment_WithDevelopment_ShouldReturnDevelopmentPreset(string envName)
    {
        // Act
        var preset = EasyMessagesPresets.ForEnvironment(envName);

        // Assert
        preset.Logging.MinimumLogLevel.Should().Be(LogLevel.Debug);
        preset.Interceptor.AutoEnrichMetadata.Should().BeTrue();
    }

    [Theory]
    [InlineData("Production")]
    [InlineData("production")]
    [InlineData("PRODUCTION")]
    public void ForEnvironment_WithProduction_ShouldReturnProductionPreset(string envName)
    {
        // Act
        var preset = EasyMessagesPresets.ForEnvironment(envName);

        // Assert
        preset.Logging.MinimumLogLevel.Should().Be(LogLevel.Warning);
        preset.Interceptor.AutoEnrichMetadata.Should().BeFalse();
        preset.Formatter.IncludeData.Should().BeFalse();
    }

    [Theory]
    [InlineData("Staging")]
    [InlineData("staging")]
    [InlineData("STAGING")]
    public void ForEnvironment_WithStaging_ShouldReturnStagingPreset(string envName)
    {
        // Act
        var preset = EasyMessagesPresets.ForEnvironment(envName);

        // Assert
        preset.Logging.MinimumLogLevel.Should().Be(LogLevel.Information);
        preset.Interceptor.AutoEnrichMetadata.Should().BeTrue();
    }

    [Theory]
    [InlineData("Testing")]
    [InlineData("Test")]
    [InlineData("test")]
    public void ForEnvironment_WithTesting_ShouldReturnTestingPreset(string envName)
    {
        // Act
        var preset = EasyMessagesPresets.ForEnvironment(envName);

        // Assert
        preset.Logging.AutoLog.Should().BeFalse();
        preset.Interceptor.AutoAddCorrelationId.Should().BeFalse();
    }

    [Theory]
    [InlineData("Unknown")]
    [InlineData("Custom")]
    [InlineData("")]
    [InlineData(null)]
    public void ForEnvironment_WithUnknown_ShouldReturnProductionPreset(string? envName)
    {
        // Act
        var preset = EasyMessagesPresets.ForEnvironment(envName!);

        // Assert - Should default to Production (safe default)
        preset.Logging.MinimumLogLevel.Should().Be(LogLevel.Warning);
        preset.Formatter.IncludeData.Should().BeFalse();
    }

    [Fact]
    public void AllPresets_ShouldHaveValidDefaultLocale()
    {
        // Act & Assert
        var presets = new[]
        {
            EasyMessagesPresets.Development,
            EasyMessagesPresets.Production,
            EasyMessagesPresets.Testing,
            EasyMessagesPresets.Staging,
            EasyMessagesPresets.Api
        };

        foreach (var preset in presets)
        {
            preset.Localization.DefaultLocale.Should().Be("en-US");
        }
    }

    [Fact]
    public void AllPresets_ShouldHaveNonNullSubOptions()
    {
        // Act & Assert
        var presets = new[]
        {
            ("Development", EasyMessagesPresets.Development),
            ("Production", EasyMessagesPresets.Production),
            ("Testing", EasyMessagesPresets.Testing),
            ("Staging", EasyMessagesPresets.Staging),
            ("Api", EasyMessagesPresets.Api)
        };

        foreach (var (name, preset) in presets)
        {
            preset.Formatter.Should().NotBeNull($"{name} preset should have Formatter");
            preset.Interceptor.Should().NotBeNull($"{name} preset should have Interceptor");
            preset.Logging.Should().NotBeNull($"{name} preset should have Logging");
            preset.Storage.Should().NotBeNull($"{name} preset should have Storage");
            preset.Localization.Should().NotBeNull($"{name} preset should have Localization");
        }
    }

    [Fact]
    public void Production_ShouldNotIncludePII()
    {
        // Act
        var preset = EasyMessagesPresets.Production;

        // Assert - Verify no PII is included
        preset.Interceptor.MetadataFields.IncludeIpAddress.Should().BeFalse("IP addresses are PII");
        preset.Interceptor.MetadataFields.IncludeUserId.Should().BeFalse("User IDs are PII");
        preset.Interceptor.MetadataFields.IncludeUserName.Should().BeFalse("User names are PII");
        preset.Interceptor.MetadataFields.IncludeUserAgent.Should().BeFalse("User agents can be used for fingerprinting");
    }

    [Fact]
    public void Development_AndProduction_ShouldDiffer()
    {
        // Act
        var dev = EasyMessagesPresets.Development;
        var prod = EasyMessagesPresets.Production;

        // Assert - They should be configured differently
        dev.Logging.MinimumLogLevel.Should().NotBe(prod.Logging.MinimumLogLevel);
        dev.Formatter.IncludeMetadata.Should().NotBe(prod.Formatter.IncludeMetadata);
        dev.Interceptor.AutoEnrichMetadata.Should().NotBe(prod.Interceptor.AutoEnrichMetadata);
    }
}
