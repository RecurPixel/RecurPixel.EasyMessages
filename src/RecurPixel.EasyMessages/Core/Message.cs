namespace RecurPixel.EasyMessages;

/// <summary>
/// Represents a message in the EasyMessages system.
/// </summary>
public sealed record Message : IMessage
{
    /// <summary>
    /// Gets the code of the message.
    /// </summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>
    /// Gets the type of the message.
    /// </summary>
    public MessageType Type { get; init; }

    // Content

    /// <summary>
    /// Gets the title of the message.
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Gets the description of the message.
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Gets the hint of the message.
    /// </summary>
    public string? Hint { get; init; }

    // Context

    /// <summary>
    /// Gets the timestamp of the message.
    /// </summary>
    public DateTime Timestamp { get; init; }

    /// <summary>
    /// Gets the correlationId of the message.
    /// </summary>
    public string? CorrelationId { get; init; }

    /// <summary>
    /// Gets the HTTP status code associated with the message.
    /// </summary>
    public int? HttpStatusCode { get; init; }

    // Data

    /// <summary>
    /// Gets the data object of the message.
    /// </summary>
    public object? Data { get; init; }

    /// <summary>
    /// Gets the parameters of the message.
    /// </summary>
    public Dictionary<string, object?> Parameters { get; init; }

    /// <summary>
    /// Gets the metadata of the message.
    /// </summary>
    public Dictionary<string, object> Metadata { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Message"/> class.
    /// </summary>
    internal Message()
    {
        Timestamp = DateTime.UtcNow;
        Parameters = new();
        Metadata = new();
    }
}
