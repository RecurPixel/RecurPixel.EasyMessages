using Microsoft.Extensions.Logging;
using RecurPixel.EasyMessages.Interceptors;

namespace RecurPixel.EasyMessages.AspNetCore.Interceptors;

public class LoggingInterceptor : IMessageInterceptor
{
    private readonly Func<ILogger> _loggerFactory;
    private ILogger? _logger;

    public LoggingInterceptor(Func<ILogger> loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    // Lazy initialization
    private ILogger? Logger => _logger ??= _loggerFactory();

    public Message OnBeforeFormat(Message message)
    {
        var logLevel = MapToLogLevel(message.Type);

        // Include correlation ID in log context
        var logState = new Dictionary<string, object>
        {
            ["Code"] = message.Code,
            ["Title"] = message.Title,
            ["Description"] = message.Description,
        };

        if (!string.IsNullOrEmpty(message.CorrelationId))
        {
            logState["CorrelationId"] = message.CorrelationId;
        }

        using (_logger?.BeginScope(logState))
        {
            _logger?.Log(
                logLevel,
                "[{Code}] {Title}: {Description}",
                message.Code,
                message.Title,
                message.Description
            );
        }

        return message;
    }

    public Message OnAfterFormat(Message message)
    {
        // No-op or log formatted result
        return message;
    }

    private static LogLevel MapToLogLevel(MessageType type) =>
        type switch
        {
            MessageType.Success => LogLevel.Information,
            MessageType.Info => LogLevel.Information,
            MessageType.Warning => LogLevel.Warning,
            MessageType.Error => LogLevel.Error,
            MessageType.Critical => LogLevel.Critical,
            _ => LogLevel.Information,
        };
}
