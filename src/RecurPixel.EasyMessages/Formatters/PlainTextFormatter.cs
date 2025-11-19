using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages.Formatters;

/// <summary>
/// Formats messages as plain text
/// </summary>
public class PlainTextFormatter : IMessageFormatter
{
    private readonly FormatterOptions _options;

    public PlainTextFormatter(FormatterOptions? options = null)
    {
        _options = options ?? FormatterOptions.Default;
    }

    public string Format(Message message)
    {
        var lines = new List<string>
        {
            $"[{message.Type.ToString().ToUpper()}] {message.Title}",
            message.Description,
        };

        if (_options.IncludeHint && !string.IsNullOrEmpty(message.Hint))
            lines.Add($"💡 Hint: {message.Hint}");

        if (_options.IncludeParameters && message.Parameters?.Count > 0)
            lines.Add(
                $"Parameters: {string.Join(", ", message.Parameters.Select(kv => $"{kv.Key}={kv.Value}"))}"
            );

        if (_options.IncludeData && message.Data != null)
            lines.Add($"Data: {message.Data}");

        lines.Add("");

        if (_options.IncludeTimestamp)
            lines.Add($"Time: {message.Timestamp:yyyy-MM-dd HH:mm:ss}");

        lines.Add($"Code: {message.Code}");

        if (_options.IncludeCorrelationId && !string.IsNullOrEmpty(message.CorrelationId))
            lines.Add($"Correlation: {message.CorrelationId}");

        return string.Join(Environment.NewLine, lines);
    }

    public object FormatAsObject(Message message) => Format(message);
}
