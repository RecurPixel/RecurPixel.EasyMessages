namespace RecurPixel.EasyMessages.AspNetCore.Configuration;

/// <summary>
/// Controls which built-in interceptors are automatically registered
/// </summary>
public class InterceptorOptions
{
    /// <summary>
    /// Automatically add correlation ID from HttpContext.TraceIdentifier
    /// </summary>
    public bool AutoAddCorrelationId { get; set; } = true;

    /// <summary>
    /// /// Automatically enrich metadata with request information
    /// </summary>
    public bool AutoEnrichMetadata { get; set; } = false;

    /// <summary>
    /// Metadata fields to include when AutoEnrichMetadata is true
    /// </summary>
    public MetadataEnrichmentFields MetadataFields { get; set; } = new();
}
