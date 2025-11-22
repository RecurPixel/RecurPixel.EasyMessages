using Microsoft.Extensions.Logging;
using RecurPixel.EasyMessages.AspNetCore.Configuration;
using RecurPixel.EasyMessages.Formatters;
using RecurPixel.EasyMessages.Interceptors;
using RecurPixel.EasyMessages.Storage;

namespace RecurPixel.EasyMessages.AspNetCore;

public class MessageConfiguration
{
    // Store configuration
    public string? CustomMessagesPath { get; set; }
    public List<IMessageStore>? CustomStores { get; set; }

    // Formatter configuration (delegates to FormatterOptions)
    public FormatterOptions FormatterOptions { get; set; } = new();

    // Formatter registration
    public Dictionary<string, Func<IMessageFormatter>>? CustomFormatters { get; set; }

    // Interceptor registration
    public List<IMessageInterceptor>? Interceptors { get; set; }

    // Interceptor behavior configuration
    public InterceptorOptions InterceptorOptions { get; set; } = new();

    // Logging configuration
    public bool AutoLog { get; set; } = false;
    public LogLevel MinimumLogLevel { get; set; } = LogLevel.Warning;

    // Localization
    public string DefaultLocale { get; set; } = "en-US";
}
