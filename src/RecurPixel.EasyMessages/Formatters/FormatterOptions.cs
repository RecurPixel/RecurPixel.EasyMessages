namespace RecurPixel.EasyMessages.Formatters;

/// <summary>
/// Options for message formatting
/// </summary>
public class FormatterOptions
{
    /// <summary>
    /// Include timestamp in the formatted output
    /// </summary>
    public bool IncludeTimestamp { get; set; } = true;

    /// <summary>
    /// Include correlation ID in the formatted output
    /// </summary>
    public bool IncludeCorrelationId { get; set; } = true;

    /// <summary>
    /// Include HTTP status code in the formatted output
    /// </summary>
    public bool IncludeHttpStatusCode { get; set; } = true;

    /// <summary>
    /// Include metadata in the formatted output
    /// </summary>
    public bool IncludeMetadata { get; set; } = true;

    /// <summary>
    /// Include data in the formatted output
    /// </summary>
    public bool IncludeData { get; set; } = true;

    /// <summary>
    /// Include parameters in the formatted output
    /// </summary>
    public bool IncludeParameters { get; set; } = true;

    /// <summary>
    /// Include hint in the formatted output
    /// </summary>
    public bool IncludeHint { get; set; } = true;

    // Null handling
    /// <summary>
    /// Include null fields in the formatted output
    /// </summary>
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
