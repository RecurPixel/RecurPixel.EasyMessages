namespace RecurPixel.EasyMessages.AspNetCore.Configuration;

/// <summary>
/// Configures which metadata fields to auto-enrich
/// </summary>
public class MetadataEnrichmentFields
{
    public bool IncludeRequestPath { get; set; } = true;
    public bool IncludeRequestMethod { get; set; } = true;
    public bool IncludeUserAgent { get; set; } = false;
    public bool IncludeIpAddress { get; set; } = false;
    public bool IncludeUserId { get; set; } = false;
    public bool IncludeUserName { get; set; } = false;
}
