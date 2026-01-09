using Microsoft.Extensions.Options;

namespace RecurPixel.EasyMessages.AspNetCore.Configuration.Validation;

/// <summary>
/// Validates EasyMessagesOptions configuration on application startup
/// </summary>
/// <remarks>
/// This validator performs comprehensive validation beyond DataAnnotations,
/// including:
/// - File path existence checks
/// - Cross-property validation
/// - Logical consistency validation
///
/// Validation occurs:
/// 1. On application startup (when ValidateOnStart is configured)
/// 2. When options are first accessed (lazy validation)
/// 3. On configuration reload (when using IOptionsMonitor)
///
/// Failed validation prevents application startup with descriptive errors.
/// </remarks>
public class EasyMessagesOptionsValidator : IValidateOptions<EasyMessagesOptions>
{
    /// <summary>
    /// Validates the options instance
    /// </summary>
    /// <param name="name">The name of the options instance (usually null for unnamed options)</param>
    /// <param name="options">The options instance to validate</param>
    /// <returns>Validation result indicating success or failure with error messages</returns>
    public ValidateOptionsResult Validate(string? name, EasyMessagesOptions options)
    {
        if (options == null)
        {
            return ValidateOptionsResult.Fail("EasyMessagesOptions cannot be null");
        }

        var failures = new List<string>();

        // Validate storage configuration
        ValidateStorage(options.Storage, failures);

        // Validate localization configuration
        ValidateLocalization(options.Localization, failures);

        // Validate logging configuration
        ValidateLogging(options.Logging, failures);

        // Validate interceptor configuration
        ValidateInterceptor(options.Interceptor, failures);

        // Return validation result
        if (failures.Any())
        {
            return ValidateOptionsResult.Fail(failures);
        }

        return ValidateOptionsResult.Success;
    }

    /// <summary>
    /// Validates storage configuration
    /// </summary>
    private void ValidateStorage(StorageOptions storage, List<string> failures)
    {
        // Validate CustomMessagesPath
        if (!string.IsNullOrEmpty(storage.CustomMessagesPath))
        {
            // Check extension first (before file existence)
            if (!storage.CustomMessagesPath.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                failures.Add($"Storage.CustomMessagesPath: File must be a JSON file (*.json), got '{storage.CustomMessagesPath}'");
            }
            else if (!File.Exists(storage.CustomMessagesPath))
            {
                failures.Add($"Storage.CustomMessagesPath: File not found at '{storage.CustomMessagesPath}'");
            }
        }

        // Validate CustomStorePaths
        if (storage.CustomStorePaths?.Any() == true)
        {
            for (int i = 0; i < storage.CustomStorePaths.Count; i++)
            {
                var path = storage.CustomStorePaths[i];

                if (string.IsNullOrWhiteSpace(path))
                {
                    failures.Add($"Storage.CustomStorePaths[{i}]: Path cannot be empty or whitespace");
                    continue;
                }

                if (!File.Exists(path))
                {
                    failures.Add($"Storage.CustomStorePaths[{i}]: File not found at '{path}'");
                }
                else if (!path.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                {
                    failures.Add($"Storage.CustomStorePaths[{i}]: File must be a JSON file (*.json), got '{path}'");
                }
            }
        }
    }

    /// <summary>
    /// Validates localization configuration
    /// </summary>
    private void ValidateLocalization(LocalizationOptions localization, List<string> failures)
    {
        // Validate DefaultLocale format
        if (string.IsNullOrWhiteSpace(localization.DefaultLocale))
        {
            failures.Add("Localization.DefaultLocale: Cannot be empty or whitespace");
        }
        else if (!System.Text.RegularExpressions.Regex.IsMatch(localization.DefaultLocale, @"^[a-z]{2}-[A-Z]{2}$"))
        {
            failures.Add($"Localization.DefaultLocale: Invalid format '{localization.DefaultLocale}'. Expected format: xx-XX (e.g., en-US, es-ES, fr-FR)");
        }

        // Warn if localization is enabled but no custom stores are configured
        if (localization.EnableLocalization)
        {
            // This is a warning, not an error, so we don't add to failures
            // In the future, you could implement a warning system
            // For now, we'll allow this configuration
        }
    }

    /// <summary>
    /// Validates logging configuration
    /// </summary>
    private void ValidateLogging(LoggingOptions logging, List<string> failures)
    {
        // Validate MinimumLogLevel is a valid enum value
        if (!Enum.IsDefined(typeof(Microsoft.Extensions.Logging.LogLevel), logging.MinimumLogLevel))
        {
            failures.Add($"Logging.MinimumLogLevel: Invalid value '{logging.MinimumLogLevel}'. Must be a valid LogLevel enum value.");
        }

        // Logical validation: If AutoLog is false, MinimumLogLevel is irrelevant
        // This is just informational, not an error
    }

    /// <summary>
    /// Validates interceptor configuration
    /// </summary>
    private void ValidateInterceptor(InterceptorOptions interceptor, List<string> failures)
    {
        // Validate metadata fields configuration
        var fields = interceptor.MetadataFields;

        // If AutoEnrichMetadata is false, metadata fields are irrelevant (but not an error)
        if (!interceptor.AutoEnrichMetadata)
        {
            // No validation needed - fields won't be used
            return;
        }

        // Check if at least one metadata field is enabled when AutoEnrichMetadata is true
        if (interceptor.AutoEnrichMetadata)
        {
            var anyFieldEnabled = fields.IncludeRequestPath
                || fields.IncludeRequestMethod
                || fields.IncludeUserAgent
                || fields.IncludeIpAddress
                || fields.IncludeUserId
                || fields.IncludeUserName;

            if (!anyFieldEnabled)
            {
                // This is a warning/info, not a hard error
                // User explicitly enabled AutoEnrichMetadata but disabled all fields
                // We'll allow this configuration (maybe they'll enable fields later)
            }
        }
    }
}
