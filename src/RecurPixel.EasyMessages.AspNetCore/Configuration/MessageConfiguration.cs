using Microsoft.Extensions.Logging;
using RecurPixel.EasyMessages.Interceptors;
using RecurPixel.EasyMessages.Storage;

namespace RecurPixel.EasyMessages.AspNetCore;

public class MessageConfiguration
{
    public string? CustomMessagesPath { get; set; }
    public string DefaultLocale { get; set; } = "en-US";
    public bool IncludeStackTrace { get; set; } = false;
    public bool IncludeTimestamp { get; set; } = true;
    public bool IncludeCorrelationId { get; set; } = true;
    public List<IMessageStore>? CustomStores { get; set; }
    public List<IMessageInterceptor>? Interceptors { get; set; }
    public bool AutoLog { get; set; } = false;
    public LogLevel MinimumLogLevel { get; set; } = LogLevel.Warning;
}
