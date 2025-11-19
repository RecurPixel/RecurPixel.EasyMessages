using RecurPixel.EasyMessages;

public interface IMessage
{
    // Core properties (Read-only for immutability)
    public string Code { get; }
    public MessageType Type { get; }
    public string Title { get; }
    public string Description { get; }
    public Dictionary<string, object> Metadata { get; }

    // Tracking
    public DateTime Timestamp { get; }
}
