using System.ComponentModel.DataAnnotations;

namespace RecurPixel.EasyMessages.AspNetCore.Configuration;

/// <summary>
/// Configuration options for message localization
/// </summary>
public class LocalizationOptions
{
    /// <summary>
    /// Default locale code for messages
    /// </summary>
    /// /// <remarks>
    /// Use standard locale format: {language}-{COUNTRY}
    ///
    /// Common examples:
    /// - en-US: English (United States)
    /// - en-GB: English (United Kingdom)
    /// - es-ES: Spanish (Spain)
    /// - fr-FR: French (France)
    /// - de-DE: German (Germany)
    /// - ja-JP: Japanese (Japan)
    ///
    /// This locale is used when:
    /// 1. Localization is disabled (EnableLocalization = false)
    /// 2. Requested locale is not available and FallbackToDefault = true
    /// 3. No locale is specified in the request
    /// </remarks>
    [RegularExpression(
        @"^[a-z]{2}-[A-Z]{2}$",
        ErrorMessage = "Locale must be in format: xx-XX (e.g., en-US)"
    )]
    public string DefaultLocale { get; set; } = "en-US";

    /// <summary>
    /// Enable localization feature
    /// </summary>
    /// <remarks>
    /// When false, all messages use DefaultLocale.
    /// When true, messages can be loaded from locale-specific files.
    ///
    /// Directory structure for localized messages:
    /// messages/
    /// ├── en-US/
    /// │   └── messages.json
    /// ├── es-ES/
    /// │   └── messages.json
    /// └── fr-FR/
    ///     └── messages.json
    ///
    /// Note: Enabling localization adds overhead. Only enable if you need
    /// multi-language support.
    /// </remarks>
    public bool EnableLocalization { get; set; } = false;

    /// <summary>
    /// Fallback to default locale if translation is missing
    /// </summary>
    /// <remarks>
    /// When true: If a message is not found in the requested locale,
    ///            fall back to DefaultLocale
    /// When false: Throw exception if message not found in requested locale
    ///
    /// Recommended: true (prevents runtime errors for incomplete translations)
    /// </remarks>
    public bool FallbackToDefault { get; set; } = true;
}
