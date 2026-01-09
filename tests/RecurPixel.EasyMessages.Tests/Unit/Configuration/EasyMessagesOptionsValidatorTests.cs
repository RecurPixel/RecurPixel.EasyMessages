using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RecurPixel.EasyMessages.AspNetCore.Configuration;
using RecurPixel.EasyMessages.AspNetCore.Configuration.Validation;

namespace RecurPixel.EasyMessages.Tests.Unit.Configuration;

public class EasyMessagesOptionsValidatorTests
{
    private readonly EasyMessagesOptionsValidator _validator;

    public EasyMessagesOptionsValidatorTests()
    {
        _validator = new EasyMessagesOptionsValidator();
    }

    [Fact]
    public void Validate_WithNullOptions_ShouldFail()
    {
        // Act
        var result = _validator.Validate(null, null!);

        // Assert
        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("cannot be null");
    }

    [Fact]
    public void Validate_WithValidOptions_ShouldSucceed()
    {
        // Arrange
        var options = new EasyMessagesOptions();

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.FailureMessage.Should().BeNull();
    }

    [Fact]
    public void Validate_WithInvalidLocale_ShouldFail()
    {
        // Arrange
        var options = new EasyMessagesOptions
        {
            Localization = new LocalizationOptions { DefaultLocale = "invalid" }
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("Localization.DefaultLocale");
        result.FailureMessage.Should().Contain("Invalid format");
        result.FailureMessage.Should().Contain("xx-XX");
    }

    [Theory]
    [InlineData("en")]
    [InlineData("EN-US")]
    [InlineData("en-us")]
    [InlineData("123-45")]
    public void Validate_WithInvalidLocaleFormats_ShouldFail(string invalidLocale)
    {
        // Arrange
        var options = new EasyMessagesOptions
        {
            Localization = new LocalizationOptions { DefaultLocale = invalidLocale }
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("Invalid format");
    }

    [Fact]
    public void Validate_WithEmptyLocale_ShouldFail()
    {
        // Arrange
        var options = new EasyMessagesOptions
        {
            Localization = new LocalizationOptions { DefaultLocale = "" }
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("Cannot be empty or whitespace");
    }

    [Theory]
    [InlineData("en-US")]
    [InlineData("es-ES")]
    [InlineData("fr-FR")]
    [InlineData("de-DE")]
    [InlineData("ja-JP")]
    public void Validate_WithValidLocaleFormats_ShouldSucceed(string validLocale)
    {
        // Arrange
        var options = new EasyMessagesOptions
        {
            Localization = new LocalizationOptions { DefaultLocale = validLocale }
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithNonExistentCustomMessagesPath_ShouldFail()
    {
        // Arrange
        var options = new EasyMessagesOptions
        {
            Storage = new StorageOptions { CustomMessagesPath = "nonexistent.json" }
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("Storage.CustomMessagesPath");
        result.FailureMessage.Should().Contain("File not found");
        result.FailureMessage.Should().Contain("nonexistent.json");
    }

    [Fact]
    public void Validate_WithNonJsonCustomMessagesPath_ShouldFail()
    {
        // Arrange
        var options = new EasyMessagesOptions
        {
            Storage = new StorageOptions { CustomMessagesPath = "test.txt" }
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("must be a JSON file");
        result.FailureMessage.Should().Contain(".json");
    }

    [Fact]
    public void Validate_WithEmptyCustomStorePath_ShouldFail()
    {
        // Arrange
        var options = new EasyMessagesOptions
        {
            Storage = new StorageOptions
            {
                CustomStorePaths = new List<string> { "" }
            }
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("Path cannot be empty");
    }

    [Fact]
    public void Validate_WithMultipleNonExistentFiles_ShouldFailWithAllErrors()
    {
        // Arrange
        var options = new EasyMessagesOptions
        {
            Storage = new StorageOptions
            {
                CustomStorePaths = new List<string>
                {
                    "missing1.json",
                    "missing2.json",
                    "missing3.json"
                }
            }
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        result.Failed.Should().BeTrue();
        result.Failures.Should().HaveCount(3);
        result.FailureMessage.Should().Contain("missing1.json");
        result.FailureMessage.Should().Contain("missing2.json");
        result.FailureMessage.Should().Contain("missing3.json");
    }

    [Fact]
    public void Validate_WithValidLogLevel_ShouldSucceed()
    {
        // Arrange
        var options = new EasyMessagesOptions
        {
            Logging = new LoggingOptions
            {
                MinimumLogLevel = LogLevel.Warning
            }
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithAutoEnrichMetadataButNoFields_ShouldStillSucceed()
    {
        // Arrange - User enabled metadata enrichment but disabled all fields
        var options = new EasyMessagesOptions
        {
            Interceptor = new InterceptorOptions
            {
                AutoEnrichMetadata = true,
                MetadataFields = new MetadataEnrichmentFields
                {
                    IncludeRequestPath = false,
                    IncludeRequestMethod = false,
                    IncludeUserAgent = false,
                    IncludeIpAddress = false,
                    IncludeUserId = false,
                    IncludeUserName = false
                }
            }
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        // This is allowed - user might enable fields later
        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithAutoEnrichMetadataDisabled_IgnoresMetadataFields()
    {
        // Arrange
        var options = new EasyMessagesOptions
        {
            Interceptor = new InterceptorOptions
            {
                AutoEnrichMetadata = false
                // MetadataFields doesn't matter when enrichment is disabled
            }
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithAutoLogDisabled_ShouldSucceed()
    {
        // Arrange
        var options = new EasyMessagesOptions
        {
            Logging = new LoggingOptions
            {
                AutoLog = false,
                MinimumLogLevel = LogLevel.Debug // Irrelevant when AutoLog is false
            }
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithNullCustomMessagesPath_ShouldSucceed()
    {
        // Arrange
        var options = new EasyMessagesOptions
        {
            Storage = new StorageOptions { CustomMessagesPath = null }
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithNullCustomStorePaths_ShouldSucceed()
    {
        // Arrange
        var options = new EasyMessagesOptions
        {
            Storage = new StorageOptions { CustomStorePaths = null }
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyCustomStorePathsList_ShouldSucceed()
    {
        // Arrange
        var options = new EasyMessagesOptions
        {
            Storage = new StorageOptions { CustomStorePaths = new List<string>() }
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithMixedValidAndInvalidLocales_AndFiles_ShouldFailWithAllErrors()
    {
        // Arrange
        var options = new EasyMessagesOptions
        {
            Storage = new StorageOptions
            {
                CustomMessagesPath = "missing.json",
                CustomStorePaths = new List<string> { "another-missing.json" }
            },
            Localization = new LocalizationOptions
            {
                DefaultLocale = "invalid-locale"
            }
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        result.Failed.Should().BeTrue();
        result.Failures.Should().HaveCountGreaterThan(1);
        result.FailureMessage.Should().Contain("missing.json");
        result.FailureMessage.Should().Contain("another-missing.json");
        result.FailureMessage.Should().Contain("invalid-locale");
    }
}
