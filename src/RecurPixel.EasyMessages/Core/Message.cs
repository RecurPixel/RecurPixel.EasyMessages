using RecurPixel.EasyMessages;

namespace RecurPixel.EasyMessages.Core;

// public sealed record Message
public sealed record Message : IMessage
{
    // Core identification
    public string Code { get; init; } = string.Empty;
    public MessageType Type { get; init; }

    // Content
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? Hint { get; init; }

    // Context
    public DateTime Timestamp { get; init; }
    public string? CorrelationId { get; init; }
    public int? HttpStatusCode { get; init; }

    // Data
    public object? Data { get; init; }
    public Dictionary<string, object>? Parameters { get; init; }
    public Dictionary<string, object>? Metadata { get; init; }

    internal Message()
    {
        Timestamp = DateTime.UtcNow;
    }
}
