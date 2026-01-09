using Microsoft.Extensions.Logging;

namespace RecurPixel.EasyMessages.AspNetCore.Configuration;

/// <summary>
/// Configuration options for automatic message logging
/// </summary>
public class LoggingOptions
{
    /// <summary>
    /// Automatically log all messages to ILogger
    /// </summary>
    /// <remarks>
    /// When enabled, messages are automatically logged based on their MessageType:
    /// - Success/Info → LogLevel.Information
    /// - Warning → LogLevel.Warning
    /// - Error → LogLevel.Error
    ///
    /// Only messages at or above MinimumLogLevel are logged.
    /// </remarks>
    public bool AutoLog { get; set; } = false;

    /// <summary>
    /// Minimum log level for automatic logging
    /// </summary>
    /// <remarks>
    /// Messages with a LogLevel below this threshold will not be logged.
    ///
    /// Recommended values:
    /// - Development: LogLevel.Debug (verbose)
    /// - Production: LogLevel.Warning (errors and warnings only)
    /// - Testing: Not applicable (AutoLog should be false)
    /// </remarks>
    public LogLevel MinimumLogLevel { get; set; } = LogLevel.Warning;
}
