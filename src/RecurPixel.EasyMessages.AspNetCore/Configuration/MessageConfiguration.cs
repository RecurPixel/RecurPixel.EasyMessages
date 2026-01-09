using Microsoft.Extensions.Logging;
using RecurPixel.EasyMessages.AspNetCore.Configuration;
using RecurPixel.EasyMessages.Formatters;
using RecurPixel.EasyMessages.Interceptors;
using RecurPixel.EasyMessages.Storage;

namespace RecurPixel.EasyMessages.AspNetCore;

/// <summary>
/// Legacy configuration class for backward compatibility
/// </summary>
/// <remarks>
/// DEPRECATED: This class is maintained for backward compatibility only.
/// Use <see cref="EasyMessagesOptions"/> instead with the new IOptions pattern.
///
/// Migration guide:
/// Old: services.AddEasyMessages(options => { ... })
/// New: services.AddEasyMessages(configuration, options => { ... })
///
/// This class will be removed in version 2.0.0.
/// </remarks>
[Obsolete(
    "Use EasyMessagesOptions with IOptions pattern instead. This class will be removed in version 2.0.0."
)]
public class MessageConfiguration
{
    // Store configuration
    /// <summary>
    /// Path to custom messages JSON file
    /// </summary>
    /// <remarks>
    /// DEPRECATED: Use EasyMessagesOptions.Storage.CustomMessagesPath instead
    /// </remarks>
    public string? CustomMessagesPath { get; set; }

    /// <summary>
    /// Custom message stores
    /// </summary>
    /// <remarks>
    /// DEPRECATED: Use EasyMessagesOptions.CustomStores instead
    /// </remarks>
    public List<IMessageStore>? CustomStores { get; set; }

    // Formatter configuration (delegates to FormatterOptions)
    /// <summary>
    /// Formatter options
    /// </summary>
    /// <remarks>
    /// DEPRECATED: Use EasyMessagesOptions.Formatter instead
    /// </remarks>
    public FormatterOptions FormatterOptions { get; set; } = new();

    // Formatter registration
    /// <summary>
    /// Custom formatters
    /// </summary>
    /// <remarks>
    /// DEPRECATED: Use EasyMessagesOptions.CustomFormatters instead
    /// </remarks>
    public Dictionary<string, Func<IMessageFormatter>>? CustomFormatters { get; set; }

    // Interceptor registration
    /// <summary>
    /// Custom interceptors
    /// </summary>
    /// <remarks>
    /// DEPRECATED: Use EasyMessagesOptions.Interceptors instead
    /// </remarks>
    public List<IMessageInterceptor>? Interceptors { get; set; }

    // Interceptor behavior configuration
    /// <summary>
    /// Interceptor options
    /// </summary>
    /// <remarks>
    /// DEPRECATED: Use EasyMessagesOptions.Interceptor instead
    /// </remarks>
    public InterceptorOptions InterceptorOptions { get; set; } = new();

    // Logging configuration
    /// <summary>
    /// Enable automatic logging
    /// </summary>
    /// <remarks>
    /// DEPRECATED: Use EasyMessagesOptions.Logging.AutoLog instead
    /// </remarks>
    public bool AutoLog { get; set; } = false;

    /// <summary>
    /// Minimum log level
    /// </summary>
    /// <remarks>
    /// DEPRECATED: Use EasyMessagesOptions.Logging.MinimumLogLevel instead
    /// </remarks>
    public LogLevel MinimumLogLevel { get; set; } = LogLevel.Warning;

    // Localization
    /// <summary>
    /// Default locale
    /// </summary>
    /// <remarks>
    /// DEPRECATED: Use EasyMessagesOptions.Localization.DefaultLocale instead
    /// </remarks>
    public string DefaultLocale { get; set; } = "en-US";

    /// <summary>
    /// Converts this legacy configuration to new EasyMessagesOptions
    /// </summary>
    /// <returns>EasyMessagesOptions with equivalent settings</returns>
    public EasyMessagesOptions ToEasyMessagesOptions()
    {
        return new EasyMessagesOptions
        {
            Formatter = FormatterOptions,
            Interceptor = InterceptorOptions,
            Logging = new LoggingOptions { AutoLog = AutoLog, MinimumLogLevel = MinimumLogLevel },
            Storage = new StorageOptions
            {
                CustomMessagesPath = CustomMessagesPath,
                CustomStorePaths = null,
            },
            Localization = new LocalizationOptions
            {
                DefaultLocale = DefaultLocale,
                EnableLocalization = false,
                FallbackToDefault = true,
            },
            CustomStores = CustomStores,
            CustomFormatters = CustomFormatters,
            Interceptors = Interceptors,
        };
    }
}
