namespace RecurPixel.EasyMessages.AspNetCore;

public class ApiResponse
{
    public bool Success { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public object? Data { get; set; }
    public DateTime Timestamp { get; set; }
    public string? CorrelationId { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}