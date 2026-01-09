using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RecurPixel.EasyMessages.AspNetCore;
using RecurPixel.EasyMessages.AspNetCore.Configuration;

namespace RecurPixel.EasyMessages.Tests.Integration.Configuration;

/// <summary>
/// Tests for configuration binding from various sources (JSON, environment variables, etc.)
/// </summary>
public class ConfigurationBindingTests
{
    [Fact]
    public void Bind_FromJsonString_ShouldWorkCorrectly()
    {
        // Arrange
        var json = @"
        {
            ""EasyMessages"": {
                ""Logging"": {
                    ""AutoLog"": true,
                    ""MinimumLogLevel"": ""Information""
                },
                ""Formatter"": {
                    ""IncludeTimestamp"": false,
                    ""IncludeCorrelationId"": true
                }
            }
        }";

        var configuration = new ConfigurationBuilder()
            .AddJsonStream(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
            .Build();

        var services = new ServiceCollection();

        // Act
        services.AddEasyMessages(configuration);
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<EasyMessagesOptions>>().Value;

        // Assert
        options.Logging.AutoLog.Should().BeTrue();
        options.Logging.MinimumLogLevel.Should().Be(Microsoft.Extensions.Logging.LogLevel.Information);
        options.Formatter.IncludeTimestamp.Should().BeFalse();
        options.Formatter.IncludeCorrelationId.Should().BeTrue();
    }

    [Fact]
    public void Bind_FromEnvironmentVariables_ShouldWorkCorrectly()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["EasyMessages:Logging:AutoLog"] = "true", // Note: : for nested keys (__ works for environment variables)
                ["EasyMessages:Logging:MinimumLogLevel"] = "Error",
                ["EasyMessages:Localization:DefaultLocale"] = "fr-FR"
            })
            .Build();

        var services = new ServiceCollection();

        // Act
        services.AddEasyMessages(configuration);
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<EasyMessagesOptions>>().Value;

        // Assert
        options.Logging.AutoLog.Should().BeTrue();
        options.Logging.MinimumLogLevel.Should().Be(Microsoft.Extensions.Logging.LogLevel.Error);
        options.Localization.DefaultLocale.Should().Be("fr-FR");
    }

    [Fact]
    public void Bind_FromMultipleSources_LastSourceWins()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["EasyMessages:Logging:AutoLog"] = "false" // First source
            })
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["EasyMessages:Logging:AutoLog"] = "true" // Second source overrides
            })
            .Build();

        var services = new ServiceCollection();

        // Act
        services.AddEasyMessages(configuration);
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<EasyMessagesOptions>>().Value;

        // Assert
        options.Logging.AutoLog.Should().BeTrue();
    }

    [Fact]
    public void Bind_CompleteConfiguration_ShouldBindAllSections()
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
            var json = $$"""
            {
                "EasyMessages": {
                    "Formatter": {
                        "IncludeTimestamp": true,
                        "IncludeCorrelationId": false,
                        "IncludeHttpStatusCode": true,
                        "IncludeMetadata": false,
                        "IncludeData": true,
                        "IncludeParameters": false,
                        "IncludeHint": true,
                        "IncludeNullFields": false
                    },
                    "Interceptor": {
                        "AutoAddCorrelationId": true,
                        "AutoEnrichMetadata": false,
                        "MetadataFields": {
                            "IncludeRequestPath": true,
                            "IncludeRequestMethod": true,
                            "IncludeUserAgent": false,
                            "IncludeIpAddress": false,
                            "IncludeUserId": false,
                            "IncludeUserName": false
                        }
                    },
                    "Logging": {
                        "AutoLog": true,
                        "MinimumLogLevel": "Warning"
                    },
                    "Storage": {
                        "CustomMessagesPath": "{{tempCustom.Replace("\\", "\\\\")}}",
                        "CustomStorePaths": [
                            "{{tempAuth.Replace("\\", "\\\\")}}",
                            "{{tempPayment.Replace("\\", "\\\\")}}"
                        ]
                    },
                    "Localization": {
                        "DefaultLocale": "es-ES",
                        "EnableLocalization": true,
                        "FallbackToDefault": true
                    }
                }
            }
            """;

            var configuration = new ConfigurationBuilder()
                .AddJsonStream(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
                .Build();

            var services = new ServiceCollection();

            // Act
            services.AddEasyMessages(configuration);
            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<EasyMessagesOptions>>().Value;

            // Assert - Formatter
            options.Formatter.IncludeTimestamp.Should().BeTrue();
            options.Formatter.IncludeCorrelationId.Should().BeFalse();
            options.Formatter.IncludeHttpStatusCode.Should().BeTrue();
            options.Formatter.IncludeMetadata.Should().BeFalse();
            options.Formatter.IncludeData.Should().BeTrue();
            options.Formatter.IncludeParameters.Should().BeFalse();
            options.Formatter.IncludeHint.Should().BeTrue();
            options.Formatter.IncludeNullFields.Should().BeFalse();

            // Assert - Interceptor
            options.Interceptor.AutoAddCorrelationId.Should().BeTrue();
            options.Interceptor.AutoEnrichMetadata.Should().BeFalse();
            options.Interceptor.MetadataFields.IncludeRequestPath.Should().BeTrue();
            options.Interceptor.MetadataFields.IncludeRequestMethod.Should().BeTrue();
            options.Interceptor.MetadataFields.IncludeUserAgent.Should().BeFalse();
            options.Interceptor.MetadataFields.IncludeIpAddress.Should().BeFalse();
            options.Interceptor.MetadataFields.IncludeUserId.Should().BeFalse();
            options.Interceptor.MetadataFields.IncludeUserName.Should().BeFalse();

            // Assert - Logging
            options.Logging.AutoLog.Should().BeTrue();
            options.Logging.MinimumLogLevel.Should().Be(Microsoft.Extensions.Logging.LogLevel.Warning);

            // Assert - Storage
            options.Storage.CustomMessagesPath.Should().Be(tempCustom);
            options.Storage.CustomStorePaths.Should().NotBeNull();
            options.Storage.CustomStorePaths.Should().HaveCount(2);
            options.Storage.CustomStorePaths![0].Should().Be(tempAuth);
            options.Storage.CustomStorePaths[1].Should().Be(tempPayment);

            // Assert - Localization
            options.Localization.DefaultLocale.Should().Be("es-ES");
            options.Localization.EnableLocalization.Should().BeTrue();
            options.Localization.FallbackToDefault.Should().BeTrue();
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
    public void Bind_PartialConfiguration_ShouldUseDefaultsForMissing()
    {
        // Arrange - Only configure Logging
        var json = @"
        {
            ""EasyMessages"": {
                ""Logging"": {
                    ""AutoLog"": true
                }
            }
        }";

        var configuration = new ConfigurationBuilder()
            .AddJsonStream(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
            .Build();

        var services = new ServiceCollection();

        // Act
        services.AddEasyMessages(configuration);
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<EasyMessagesOptions>>().Value;

        // Assert - Configured value
        options.Logging.AutoLog.Should().BeTrue();

        // Assert - Defaults for unconfigured values
        options.Logging.MinimumLogLevel.Should().Be(Microsoft.Extensions.Logging.LogLevel.Warning);
        options.Formatter.IncludeTimestamp.Should().BeTrue();
        options.Interceptor.AutoAddCorrelationId.Should().BeTrue();
        options.Localization.DefaultLocale.Should().Be("en-US");
    }

    [Theory]
    [InlineData("Debug", Microsoft.Extensions.Logging.LogLevel.Debug)]
    [InlineData("Information", Microsoft.Extensions.Logging.LogLevel.Information)]
    [InlineData("Warning", Microsoft.Extensions.Logging.LogLevel.Warning)]
    [InlineData("Error", Microsoft.Extensions.Logging.LogLevel.Error)]
    [InlineData("Critical", Microsoft.Extensions.Logging.LogLevel.Critical)]
    [InlineData("None", Microsoft.Extensions.Logging.LogLevel.None)]
    public void Bind_LogLevelFromString_ShouldConvertCorrectly(string logLevelString, Microsoft.Extensions.Logging.LogLevel expectedLevel)
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["EasyMessages:Logging:MinimumLogLevel"] = logLevelString
            })
            .Build();

        var services = new ServiceCollection();

        // Act
        services.AddEasyMessages(configuration);
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<EasyMessagesOptions>>().Value;

        // Assert
        options.Logging.MinimumLogLevel.Should().Be(expectedLevel);
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    [InlineData("True", true)]
    [InlineData("False", false)]
    [InlineData("TRUE", true)]
    [InlineData("FALSE", false)]
    public void Bind_BooleanValues_ShouldBeCaseInsensitive(string boolString, bool expectedValue)
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["EasyMessages:Logging:AutoLog"] = boolString
            })
            .Build();

        var services = new ServiceCollection();

        // Act
        services.AddEasyMessages(configuration);
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<EasyMessagesOptions>>().Value;

        // Assert
        options.Logging.AutoLog.Should().Be(expectedValue);
    }

    [Fact]
    public void Bind_ArrayValues_ShouldBindCorrectly()
    {
        // Arrange - Create temporary test files
        var tempFiles = new[]
        {
            Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json"),
            Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json"),
            Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json")
        };

        try
        {
            // Create the files
            foreach (var file in tempFiles)
            {
                File.WriteAllText(file, "{}");
            }

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["EasyMessages:Storage:CustomStorePaths:0"] = tempFiles[0],
                    ["EasyMessages:Storage:CustomStorePaths:1"] = tempFiles[1],
                    ["EasyMessages:Storage:CustomStorePaths:2"] = tempFiles[2]
                })
                .Build();

            var services = new ServiceCollection();

            // Act
            services.AddEasyMessages(configuration);
            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<EasyMessagesOptions>>().Value;

            // Assert
            options.Storage.CustomStorePaths.Should().NotBeNull();
            options.Storage.CustomStorePaths.Should().HaveCount(3);
            options.Storage.CustomStorePaths![0].Should().Be(tempFiles[0]);
            options.Storage.CustomStorePaths[1].Should().Be(tempFiles[1]);
            options.Storage.CustomStorePaths[2].Should().Be(tempFiles[2]);
        }
        finally
        {
            // Cleanup temp files
            foreach (var file in tempFiles)
            {
                if (File.Exists(file))
                    File.Delete(file);
            }
        }
    }

    [Fact]
    public void Bind_EmptySection_ShouldUseDefaults()
    {
        // Arrange - EasyMessages section exists but is empty
        var json = @"
        {
            ""EasyMessages"": {
            }
        }";

        var configuration = new ConfigurationBuilder()
            .AddJsonStream(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
            .Build();

        var services = new ServiceCollection();

        // Act
        services.AddEasyMessages(configuration);
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<EasyMessagesOptions>>().Value;

        // Assert - Should have all defaults
        options.Logging.AutoLog.Should().BeFalse();
        options.Logging.MinimumLogLevel.Should().Be(Microsoft.Extensions.Logging.LogLevel.Warning);
        options.Formatter.IncludeTimestamp.Should().BeTrue();
        options.Localization.DefaultLocale.Should().Be("en-US");
    }

    [Fact]
    public void Bind_MissingSection_ShouldUseDefaults()
    {
        // Arrange - No EasyMessages section at all
        var json = @"
        {
            ""SomeOtherConfig"": {
                ""Value"": ""test""
            }
        }";

        var configuration = new ConfigurationBuilder()
            .AddJsonStream(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
            .Build();

        var services = new ServiceCollection();

        // Act
        services.AddEasyMessages(configuration);
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<EasyMessagesOptions>>().Value;

        // Assert - Should have all defaults
        options.Logging.AutoLog.Should().BeFalse();
        options.Formatter.IncludeTimestamp.Should().BeTrue();
        options.Localization.DefaultLocale.Should().Be("en-US");
    }
}
