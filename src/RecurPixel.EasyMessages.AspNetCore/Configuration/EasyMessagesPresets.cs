using Microsoft.Extensions.Logging;
using RecurPixel.EasyMessages.Configuration;

namespace RecurPixel.EasyMessages.AspNetCore.Configuration;

/// <summary>
/// Pre-configured option sets for common scenarios
/// </summary>
/// <remarks>
/// Presets provide ready-to-use configurations for typical development scenarios,
/// eliminating the need to manually configure every option.
///
/// Available presets:
/// - Development: Verbose output for local development and debugging
/// - Production: Privacy-safe configuration optimized for production
/// - Testing: Minimal configuration for unit/integration tests
/// - Staging: Balanced configuration for staging/QA environments
///
/// Usage:
/// services.AddEasyMessages(configuration, EasyMessagesPresets.Development);
/// services.AddEasyMessages(configuration, EasyMessagesPresets.Production);
///
/// You can also use presets as a starting point and customize:
/// services.AddEasyMessages(configuration, options =>
/// {
///     // Start with preset
///     var preset = EasyMessagesPresets.Production;
///     options.Formatter = preset.Formatter;
///     options.Logging = preset.Logging;
///
///     // Customize specific settings
///     options.Logging.MinimumLogLevel = LogLevel.Information;
/// });
/// </remarks>
public static class EasyMessagesPresets
{
    /// <summary>
    /// Development preset: Verbose logging and debugging
    /// </summary>
    /// <remarks>
    /// Optimized for local development and troubleshooting.
    ///
    /// Configuration:
    /// - Formatter: Debug (includes everything, even null fields)
    /// - Logging: AutoLog enabled with Debug level
    /// - Interceptor: All enrichment enabled
    /// - Metadata: All fields included
    ///
    /// Use when:
    /// - Local development
    /// - Troubleshooting issues
    /// - Understanding message flow
    ///
    /// Warning: Includes all data, metadata, and request information.
    ///          Do not use in production due to potential sensitive data exposure.
    /// </remarks>
    public static EasyMessagesOptions Development =>
        new()
        {
            Formatter = FormatterConfiguration.Debug,
            Logging = new LoggingOptions { AutoLog = true, MinimumLogLevel = LogLevel.Debug },
            Interceptor = new InterceptorOptions
            {
                AutoAddCorrelationId = true,
                AutoEnrichMetadata = true,
                MetadataFields = new MetadataEnrichmentFields
                {
                    IncludeRequestPath = true,
                    IncludeRequestMethod = true,
                    IncludeUserAgent = true,
                    IncludeIpAddress = true,
                    IncludeUserId = true,
                    IncludeUserName = true,
                },
            },
            Storage = new StorageOptions(),
            Localization = new LocalizationOptions(),
        };

    /// <summary>
    /// Production preset: Privacy-safe, optimized configuration
    /// </summary>
    /// <remarks>
    /// Optimized for production deployments with privacy and security in mind.
    ///
    /// Configuration:
    /// - Formatter: ProductionSafe (excludes sensitive data)
    /// - Logging: AutoLog enabled with Warning level
    /// - Interceptor: Correlation ID only, no metadata enrichment
    /// - Metadata: Not included
    ///
    /// Use when:
    /// - Production deployments
    /// - Privacy/GDPR compliance required
    /// - Minimal information exposure needed
    ///
    /// Benefits:
    /// - No PII (Personal Identifiable Information) exposure
    /// - Reduced log volume
    /// - Still captures errors and warnings
    /// - Correlation ID for distributed tracing
    /// </remarks>
    public static EasyMessagesOptions Production =>
        new()
        {
            Formatter = FormatterConfiguration.ProductionSafe,
            Logging = new LoggingOptions { AutoLog = true, MinimumLogLevel = LogLevel.Warning },
            Interceptor = new InterceptorOptions
            {
                AutoAddCorrelationId = true,
                AutoEnrichMetadata = false,
                MetadataFields = new MetadataEnrichmentFields
                {
                    IncludeRequestPath = false,
                    IncludeRequestMethod = false,
                    IncludeUserAgent = false,
                    IncludeIpAddress = false,
                    IncludeUserId = false,
                    IncludeUserName = false,
                },
            },
            Storage = new StorageOptions(),
            Localization = new LocalizationOptions(),
        };

    /// <summary>
    /// Testing preset: Minimal configuration for tests
    /// </summary>
    /// <remarks>
    /// Optimized for unit tests and integration tests.
    ///
    /// Configuration:
    /// - Formatter: Minimal (only essential fields)
    /// - Logging: Disabled (no log noise in tests)
    /// - Interceptor: All features disabled
    /// - Metadata: Not included
    ///
    /// Use when:
    /// - Running unit tests
    /// - Running integration tests
    /// - Testing message behavior in isolation
    ///
    /// Benefits:
    /// - No side effects (no logging, no interceptors)
    /// - Minimal output for easier assertions
    /// - Predictable behavior
    /// - Fast execution
    /// </remarks>
    public static EasyMessagesOptions Testing =>
        new()
        {
            Formatter = FormatterConfiguration.Minimal,
            Logging = new LoggingOptions { AutoLog = false, MinimumLogLevel = LogLevel.None },
            Interceptor = new InterceptorOptions
            {
                AutoAddCorrelationId = false,
                AutoEnrichMetadata = false,
                MetadataFields = new MetadataEnrichmentFields
                {
                    IncludeRequestPath = false,
                    IncludeRequestMethod = false,
                    IncludeUserAgent = false,
                    IncludeIpAddress = false,
                    IncludeUserId = false,
                    IncludeUserName = false,
                },
            },
            Storage = new StorageOptions(),
            Localization = new LocalizationOptions(),
        };

    /// <summary>
    /// Staging preset: Balanced configuration for pre-production
    /// </summary>
    /// <remarks>
    /// Balanced between debugging capabilities and production readiness.
    ///
    /// Configuration:
    /// - Formatter: Verbose (includes most fields)
    /// - Logging: AutoLog enabled with Information level
    /// - Interceptor: Correlation ID and metadata enrichment enabled
    /// - Metadata: Basic fields included (path, method), no PII
    ///
    /// Use when:
    /// - Staging environment
    /// - QA testing
    /// - Pre-production validation
    /// - Load testing
    ///
    /// Benefits:
    /// - More detailed than production for troubleshooting
    /// - Less verbose than development for performance
    /// - Basic metadata without PII concerns
    /// - Correlation tracking for distributed tracing
    /// </remarks>
    public static EasyMessagesOptions Staging =>
        new()
        {
            Formatter = FormatterConfiguration.Verbose,
            Logging = new LoggingOptions { AutoLog = true, MinimumLogLevel = LogLevel.Information },
            Interceptor = new InterceptorOptions
            {
                AutoAddCorrelationId = true,
                AutoEnrichMetadata = true,
                MetadataFields = new MetadataEnrichmentFields
                {
                    IncludeRequestPath = true,
                    IncludeRequestMethod = true,
                    IncludeUserAgent = false,
                    IncludeIpAddress = false,
                    IncludeUserId = false,
                    IncludeUserName = false,
                },
            },
            Storage = new StorageOptions(),
            Localization = new LocalizationOptions(),
        };

    /// <summary>
    /// API preset: Clean responses optimized for API consumers
    /// </summary>
    /// <remarks>
    /// Optimized for public API responses where clients need clean, concise messages.
    ///
    /// Configuration:
    /// - Formatter: ApiClient (clean, essential fields)
    /// - Logging: Disabled (log separately if needed)
    /// - Interceptor: Correlation ID for tracing, no metadata
    /// - Metadata: Not included in responses
    ///
    /// Use when:
    /// - Building public APIs
    /// - REST API responses
    /// - Client-facing services
    /// - Mobile/web application backends
    ///
    /// Benefits:
    /// - Clean, minimal JSON responses
    /// - No internal implementation details exposed
    /// - Includes hints for user guidance
    /// - Includes data payloads
    /// - Correlation ID for support purposes
    /// </remarks>
    public static EasyMessagesOptions Api =>
        new()
        {
            Formatter = FormatterConfiguration.ApiClient,
            Logging = new LoggingOptions { AutoLog = false, MinimumLogLevel = LogLevel.None },
            Interceptor = new InterceptorOptions
            {
                AutoAddCorrelationId = true,
                AutoEnrichMetadata = false,
                MetadataFields = new MetadataEnrichmentFields(),
            },
            Storage = new StorageOptions(),
            Localization = new LocalizationOptions(),
        };

    /// <summary>
    /// Creates a custom preset based on the current environment
    /// </summary>
    /// <param name="environmentName">Environment name (Development, Production, Staging, etc.)</param>
    /// <returns>EasyMessagesOptions configured for the environment</returns>
    /// <remarks>
    /// Automatically selects the appropriate preset based on environment name.
    ///
    /// Mapping:
    /// - "Development" → Development preset
    /// - "Production" → Production preset
    /// - "Staging" → Staging preset
    /// - "Testing" / "Test" → Testing preset
    /// - Other → Production preset (safe default)
    ///
    /// Usage:
    /// var options = EasyMessagesPresets.ForEnvironment(env.EnvironmentName);
    /// services.AddEasyMessages(configuration, options);
    /// </remarks>
    public static EasyMessagesOptions ForEnvironment(string environmentName)
    {
        return environmentName?.ToLowerInvariant() switch
        {
            "development" => Development,
            "production" => Production,
            "staging" => Staging,
            "testing" or "test" => Testing,
            _ => Production, // Safe default
        };
    }
}
