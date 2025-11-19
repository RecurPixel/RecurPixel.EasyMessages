namespace RecurPixel.EasyMessages.Formatters;

public class FormatterOptions
{
    public bool IncludeTimestamp { get; set; } = true;
    public bool IncludeCorrelationId { get; set; } = true;
    public bool IncludeHttpStatusCode { get; set; } = true;
    public bool IncludeMetadata { get; set; } = true;
    public bool IncludeData { get; set; } = true;
    public bool IncludeParameters { get; set; } = true;
    public bool IncludeHint { get; set; } = true;

    // Null handling
    public bool IncludeNullFields { get; set; } = false;

    public static FormatterOptions Default => new();
    public static FormatterOptions Minimal =>
        new()
        {
            IncludeTimestamp = false,
            IncludeCorrelationId = false,
            IncludeHttpStatusCode = false,
            IncludeMetadata = false,
            IncludeParameters = false,
            IncludeHint = false,
        };
}
