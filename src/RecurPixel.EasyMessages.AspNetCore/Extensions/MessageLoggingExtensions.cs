
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace RecurPixel.EasyMessages.AspNetCore;

public static class LoggingExtensions
{
    public static Message Log(this Message message, ILogger? logger = null)
    {
        logger ??= GetDefaultLogger();
        
        var logLevel = MapToLogLevel(message.Type);
        
        logger.Log(logLevel,
            "[{Code}] {Title}: {Description}",
            message.Code,
            message.Title,
            message.Description);
        
        return message;
    }
    
    private static LogLevel MapToLogLevel(MessageType type) => type switch
    {
        MessageType.Success => LogLevel.Information,
        MessageType.Info => LogLevel.Information,
        MessageType.Warning => LogLevel.Warning,
        MessageType.Error => LogLevel.Error,
        MessageType.Critical => LogLevel.Critical,
        _ => LogLevel.Information
    };
    
    private static ILogger GetDefaultLogger()
    {
        // Return null logger if no DI setup
        return NullLogger.Instance;
    }
}