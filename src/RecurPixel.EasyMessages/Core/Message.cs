namespace RecurPixel.EasyMessages.Core;

// public sealed record Message
public sealed record Message
{
    // Core properties
    public string Code { get; init; }
    public MessageType Type { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    
    // Optional context
    public object? Data { get; init; }
    public Dictionary<string, object>? Metadata { get; init; }
    
    // Tracking
    public DateTime Timestamp { get; init; }
    public string? CorrelationId { get; init; }
    
    // HTTP
    public int? HttpStatusCode { get; init; }

    // Hint
    public string? Hint { get; set; }
    
    // Internal constructor - force creation through MessageRegistry
    internal Message()
    {
        Timestamp = DateTime.UtcNow;

        Metadata = new Dictionary<string, object>();
    }
}