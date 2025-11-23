using RecurPixel.EasyMessages.Formatters;

namespace RecurPixel.EasyMessages.Configuration;

/// <summary>
/// Configuration for global formatter options
/// </summary>
public static class FormatterConfiguration
{
    /// <summary>
    /// The global default formatter options
    /// </summary>
    private static FormatterOptions _defaultOptions = new();

    /// <summary>
    /// Lock object for thread safety
    /// </summary>
    private static readonly object _lock = new();

    /// <summary>
    /// /// Get the global default formatter options
    /// </summary>
    public static FormatterOptions DefaultOptions
    {
        get
        {
            lock (_lock)
            {
                return _defaultOptions;
            }
        }
    }

    /// <summary>
    /// Set the global default formatter options
    /// </summary>
    public static void SetDefaultOptions(FormatterOptions options)
    {
        lock (_lock)
        {
            _defaultOptions = options ?? throw new ArgumentNullException(nameof(options));
        }
    }

    /// <summary>
    /// Configure global formatter options
    /// </summary>
    public static void Configure(Action<FormatterOptions> configure)
    {
        lock (_lock)
        {
            configure?.Invoke(_defaultOptions);
        }
    }

    /// <summary>
    /// Reset to default options
    /// </summary>
    public static void Reset()
    {
        lock (_lock)
        {
            _defaultOptions = new FormatterOptions();
        }
    }

    /// <summary>
    /// Preset: Minimal output (only essential fields)
    /// </summary>
    public static FormatterOptions Minimal =>
        new()
        {
            IncludeTimestamp = false,
            IncludeCorrelationId = false,
            IncludeHttpStatusCode = false,
            IncludeMetadata = false,
            IncludeData = true, // Keep data - it's usually important
            IncludeParameters = false,
            IncludeHint = false,
            IncludeNullFields = false,
        };

    /// <summary>
    /// Preset: Verbose output (include everything)
    /// </summary>
    public static FormatterOptions Verbose =>
        new()
        {
            IncludeTimestamp = true,
            IncludeCorrelationId = true,
            IncludeHttpStatusCode = true,
            IncludeMetadata = true,
            IncludeData = true,
            IncludeParameters = true,
            IncludeHint = true,
            IncludeNullFields = true, // Even include null fields
        };

    /// <summary>
    /// Preset: Production-safe output (no sensitive data)
    /// </summary>
    public static FormatterOptions ProductionSafe =>
        new()
        {
            IncludeTimestamp = true,
            IncludeCorrelationId = true,
            IncludeHttpStatusCode = true,
            IncludeMetadata = false, // May contain sensitive data
            IncludeData = false, // May contain sensitive data
            IncludeParameters = false, // May contain sensitive data
            IncludeHint = true,
            IncludeNullFields = false,
        };

    /// <summary>
    /// Preset: Debug output (everything for troubleshooting)
    /// </summary>
    public static FormatterOptions Debug =>
        new()
        {
            IncludeTimestamp = true,
            IncludeCorrelationId = true,
            IncludeHttpStatusCode = true,
            IncludeMetadata = true,
            IncludeData = true,
            IncludeParameters = true,
            IncludeHint = true,
            IncludeNullFields = true,
        };

    /// <summary>
    /// Preset: API client response (clean, essential fields only)
    /// </summary>
    public static FormatterOptions ApiClient =>
        new()
        {
            IncludeTimestamp = false,
            IncludeCorrelationId = false,
            IncludeHttpStatusCode = true, // Clients need this
            IncludeMetadata = false,
            IncludeData = true, // Clients need this
            IncludeParameters = false,
            IncludeHint = true, // Helpful for users
            IncludeNullFields = false,
        };

    /// <summary>
    /// Preset: Logging output (context for log analysis)
    /// </summary>
    public static FormatterOptions Logging =>
        new()
        {
            IncludeTimestamp = true, // Essential for logs
            IncludeCorrelationId = true, // Essential for logs
            IncludeHttpStatusCode = true,
            IncludeMetadata = true, // Context for analysis
            IncludeData = false, // May be too verbose for logs
            IncludeParameters = true, // Context for analysis
            IncludeHint = false, // Not needed in logs
            IncludeNullFields = false,
        };
}
