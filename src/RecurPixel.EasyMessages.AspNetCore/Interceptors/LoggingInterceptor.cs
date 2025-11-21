using Microsoft.Extensions.Logging;
using RecurPixel.EasyMessages.Interceptors;

namespace RecurPixel.EasyMessages.AspNetCore.Interceptors;

public class LoggingInterceptor : IMessageInterceptor
{
    private readonly ILogger _logger;

    public LoggingInterceptor(ILogger logger)
    {
        _logger = logger;
    }

    public Message OnBeforeFormat(Message message)
    {
        var logLevel = MapToLogLevel(message.Type);

        _logger.Log(
            logLevel,
            "[{Code}] {Title}: {Description}",
            message.Code,
            message.Title,
            message.Description
        );

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
