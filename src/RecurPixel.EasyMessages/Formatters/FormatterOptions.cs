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

    /// <summary>
    /// Create a deep copy of these options
    /// </summary>
    public FormatterOptions Clone()
    {
        return new FormatterOptions
        {
            IncludeTimestamp = IncludeTimestamp,
            IncludeCorrelationId = IncludeCorrelationId,
            IncludeHttpStatusCode = IncludeHttpStatusCode,
            IncludeMetadata = IncludeMetadata,
            IncludeData = IncludeData,
            IncludeParameters = IncludeParameters,
            IncludeHint = IncludeHint,
            IncludeNullFields = IncludeNullFields,
        };
    }
}
