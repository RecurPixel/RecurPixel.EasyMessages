namespace RecurPixel.EasyMessages.Core;

/// <summary>
/// Defines a template for creating Message objects.
/// </summary>
public sealed class MessageTemplate
{
    /// <summary>
    /// Gets or sets the type of the message.
    /// </summary>
    public MessageType Type { get; set; }

    /// <summary>
    /// Gets or sets the title of the message.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the description of the message.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the HTTP status code associated with the message.
    /// </summary>
    public int? HttpStatusCode { get; set; }

    /// <summary>
    /// Gets or sets the hint of the message.
    /// </summary>
    public string? Hint { get; set; }

    /// <summary>
    /// Converts the MessageTemplate to a Message instance with the specified code.
    /// </summary>
    /// <param name="code">Message Code</param>
    /// <returns>Message Object</returns>
    public Message ToMessage(string code)
    {
        return new Message
        {
            Code = code,
            Type = Type,
            Title = Title ?? string.Empty,
            Description = Description ?? string.Empty,
            HttpStatusCode = HttpStatusCode,
            Hint = Hint,
        };
    }
}
