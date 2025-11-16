namespace RecurPixel.EasyMessages.Configuration;

public class MessageConfiguration
{
    public string? CustomMessagesPath { get; set; }
    public string DefaultLocale { get; set; } = "en-US";
    public bool IncludeStackTrace { get; set; } = false;
    public bool IncludeTimestamp { get; set; } = true;
    public bool IncludeCorrelationId { get; set; } = true;
    public bool AutoLog { get; set; } = false;
    // public LogLevel MinimumLogLevel { get; set; } = LogLevel.Warning;
}
