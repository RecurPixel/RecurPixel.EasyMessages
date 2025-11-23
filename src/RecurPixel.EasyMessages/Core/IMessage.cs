using RecurPixel.EasyMessages;

/// <summary>
/// Defines the interface for a message object in the EasyMessages system.
/// </summary>
public interface IMessage
{
    // Core properties (Read-only for immutability)

    /// <summary>
    /// Gets the code of the message.
    /// </summary>
    public string Code { get; }
    /// <summary>
    /// Gets the type of the message.
    /// </summary>
    /// <value>Returns MessageType Enum Value</value>
    public MessageType Type { get; }
    /// <summary>
    /// Gets the title of the message.
    /// </summary>
    /// <value>Returns Title string</value>
    public string Title { get; }
    /// <summary>
    /// Gets the description of the message.
    /// </summary>   
    /// /// <value>Returns Description string</value>
    public string Description { get; }
    /// <summary>
    /// Gets the hint of the message.
    /// </summary>
    /// <value>Returns Hint string</value>
    public Dictionary<string, object> Metadata { get; }

    // Tracking

    /// <summary>
    /// Gets the timestamp of the message.
    /// </summary>
    /// <value>Returns Timestamp DateTime</value>
    public DateTime Timestamp { get; }
}
