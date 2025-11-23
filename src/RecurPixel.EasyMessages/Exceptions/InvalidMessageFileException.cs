namespace RecurPixel.EasyMessages.Exceptions;

/// <summary>
/// Exception thrown when a message file is invalid.
/// </summary>
public class InvalidMessageFileException : EasyMessagesException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidMessageFileException"/> class.
    /// </summary>
    /// <param name="message">Message String</param>
    public InvalidMessageFileException(string message)
        : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidMessageFileException"/> class with an inner exception.
    /// </summary>
    /// <param name="message">Message String</param>
    /// <param name="innerException">Inner Exception</param>
    public InvalidMessageFileException(string message, Exception innerException)
        : base(message, innerException) { }
}
