using System.ComponentModel.DataAnnotations;
using RecurPixel.EasyMessages.Formatters;
using RecurPixel.EasyMessages.Interceptors;
using RecurPixel.EasyMessages.Storage;

namespace RecurPixel.EasyMessages.AspNetCore.Configuration;

/// <summary>
/// Root configuration options for EasyMessages library
/// </summary>
/// <remarks>
/// This is the main configuration class that aggregates all EasyMessages settings.
/// It supports configuration from:
/// 1. appsettings.json (via IConfiguration binding)
/// 2. Programmatic configuration (via Action delegate)
/// 3. Presets (via EasyMessagesPresets)
///
/// Configuration sections in appsettings.json:
/// {
///   "EasyMessages": {
///     "Formatter": { ... },
///     "Interceptor": { ... },
///     "Logging": { ... },
///     "Storage": { ... },
///     "Localization": { ... }
///   }
/// }
///
/// See CONFIGURATION_GUIDE.md for detailed documentation.
/// </remarks>
public class EasyMessagesOptions : IValidatableObject
{
    /// <summary>
    /// Configuration section name in appsettings.json
    /// </summary>
    public const string SectionName = "EasyMessages";

    /// <summary>
    /// Message formatting options
    /// </summary>
    /// <remarks>
    /// Controls which fields are included in formatted output (JSON, XML, Console).
    /// Common presets available: Minimal, Verbose, ProductionSafe, Debug, ApiClient, Logging
    /// </remarks>
    [Required]
    public FormatterOptions Formatter { get; set; } = new();

    /// <summary>
    /// Interceptor behavior options
    /// </summary>
    /// <remarks>
    /// Controls automatic message enrichment:
    /// - AutoAddCorrelationId: Add correlation ID from HttpContext
    /// - AutoEnrichMetadata: Add request metadata (path, method, user, etc.)
    /// </remarks>
    [Required]
    public InterceptorOptions Interceptor { get; set; } = new();

    /// <summary>
    /// Automatic logging options
    /// </summary>
    /// <remarks>
    /// Configure automatic logging of messages through ILogger:
    /// - AutoLog: Enable/disable automatic logging
    /// - MinimumLogLevel: Only log messages at or above this level
    /// </remarks>
    [Required]
    public LoggingOptions Logging { get; set; } = new();

    /// <summary>
    /// Custom message storage options
    /// </summary>
    /// <remarks>
    /// Configure where to load custom messages from:
    /// - CustomMessagesPath: Single JSON file path
    /// - CustomStorePaths: Multiple JSON file paths
    /// Later sources override earlier ones for duplicate codes.
    /// </remarks>
    [Required]
    public StorageOptions Storage { get; set; } = new();

    /// <summary>
    /// Localization options
    /// </summary>
    /// <remarks>
    /// Configure multi-language message support:
    /// - DefaultLocale: Default language (e.g., "en-US")
    /// - EnableLocalization: Turn on/off localization feature
    /// - FallbackToDefault: Fall back to default locale if translation missing
    /// </remarks>
    [Required]
    public LocalizationOptions Localization { get; set; } = new();

    // Advanced configuration (not typically set via appsettings.json)

    /// <summary>
    /// Custom message stores (programmatic registration only)
    /// </summary>
    /// <remarks>
    /// Register custom IMessageStore implementations programmatically.
    /// Not configurable via appsettings.json (requires code).
    ///
    /// Example:
    /// options.CustomStores = new List&lt;IMessageStore&gt;
    /// {
    ///     new DatabaseMessageStore(connectionString),
    ///     new RedisMessageStore(redisConnection)
    /// };
    /// </remarks>
    public List<IMessageStore>? CustomStores { get; set; }

    /// <summary>
    /// Custom formatter registration (programmatic registration only)
    /// </summary>
    /// <remarks>
    /// Register custom IMessageFormatter implementations programmatically.
    /// Not configurable via appsettings.json (requires code).
    ///
    /// Example:
    /// options.CustomFormatters = new Dictionary&lt;string, Func&lt;IMessageFormatter&gt;&gt;
    /// {
    ///     ["csv"] = () => new CsvFormatter(),
    ///     ["markdown"] = () => new MarkdownFormatter()
    /// };
    /// </remarks>
    public Dictionary<string, Func<IMessageFormatter>>? CustomFormatters { get; set; }

    /// <summary>
    /// Custom interceptors (programmatic registration only)
    /// </summary>
    /// <remarks>
    /// Register custom IMessageInterceptor implementations programmatically.
    /// Not configurable via appsettings.json (requires code).
    ///
    /// Example:
    /// options.Interceptors = new List&lt;IMessageInterceptor&gt;
    /// {
    ///     new CustomLoggingInterceptor(),
    ///     new MetricsInterceptor()
    /// };
    /// </remarks>
    public List<IMessageInterceptor>? Interceptors { get; set; }

    /// <summary>
    /// Validates the configuration options
    /// </summary>
    /// <param name="validationContext">Validation context</param>
    /// <returns>Collection of validation results</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errors = new List<ValidationResult>();

        // Note: File existence validation is handled by EasyMessagesOptionsValidator
        // This keeps DataAnnotations validation lightweight and allows for testing scenarios
        // with non-existent files

        // Validate DefaultLocale format (should be validated by RegularExpression attribute, but double-check)
        if (
            !System.Text.RegularExpressions.Regex.IsMatch(
                Localization.DefaultLocale,
                @"^[a-z]{2}-[A-Z]{2}$"
            )
        )
        {
            errors.Add(
                new ValidationResult(
                    $"Invalid locale format: {Localization.DefaultLocale}. Expected format: xx-XX (e.g., en-US)",
                    new[] { $"{nameof(Localization)}.{nameof(Localization.DefaultLocale)}" }
                )
            );
        }

        return errors;
    }

    /// <summary>
    /// Creates a deep copy of this options instance
    /// </summary>
    /// <returns>A new EasyMessagesOptions instance with the same values</returns>
    public EasyMessagesOptions Clone()
    {
        return new EasyMessagesOptions
        {
            Formatter = Formatter.Clone(),
            Interceptor = new InterceptorOptions
            {
                AutoAddCorrelationId = Interceptor.AutoAddCorrelationId,
                AutoEnrichMetadata = Interceptor.AutoEnrichMetadata,
                MetadataFields = new MetadataEnrichmentFields
                {
                    IncludeRequestPath = Interceptor.MetadataFields.IncludeRequestPath,
                    IncludeRequestMethod = Interceptor.MetadataFields.IncludeRequestMethod,
                    IncludeUserAgent = Interceptor.MetadataFields.IncludeUserAgent,
                    IncludeIpAddress = Interceptor.MetadataFields.IncludeIpAddress,
                    IncludeUserId = Interceptor.MetadataFields.IncludeUserId,
                    IncludeUserName = Interceptor.MetadataFields.IncludeUserName,
                },
            },
            Logging = new LoggingOptions
            {
                AutoLog = Logging.AutoLog,
                MinimumLogLevel = Logging.MinimumLogLevel,
            },
            Storage = new StorageOptions
            {
                CustomMessagesPath = Storage.CustomMessagesPath,
                CustomStorePaths = Storage.CustomStorePaths?.ToList(),
            },
            Localization = new LocalizationOptions
            {
                DefaultLocale = Localization.DefaultLocale,
                EnableLocalization = Localization.EnableLocalization,
                FallbackToDefault = Localization.FallbackToDefault,
            },
            CustomStores = CustomStores?.ToList(),
            CustomFormatters = CustomFormatters?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
            Interceptors = Interceptors?.ToList(),
        };
    }
}
