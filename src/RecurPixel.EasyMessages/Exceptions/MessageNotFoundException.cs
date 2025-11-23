namespace RecurPixel.EasyMessages.Exceptions;

/// <summary>
/// Exception thrown when a message is not found.
/// </summary>
public class MessageNotFoundException : EasyMessagesException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MessageNotFoundException"/> class.
    /// </summary>
    /// <param name="message">Message String</param>
    public MessageNotFoundException(string message)
        : base(message) { }

    
    /// <summary>
    /// Initializes a new instance of the <see cref="MessageNotFoundException"/> class with an inner exception.
    /// </summary>
    /// <param name="message">Message String</param>
    /// <param name="innerException">Inner Exception</param>
    public MessageNotFoundException(string message, Exception innerException)
        : base(message, innerException) { }
}
