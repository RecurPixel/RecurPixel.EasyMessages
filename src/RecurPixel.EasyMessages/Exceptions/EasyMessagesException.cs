namespace RecurPixel.EasyMessages.Exceptions;

/// <summary>
/// Base exception class for EasyMessages-related errors.
/// </summary>
public class EasyMessagesException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EasyMessagesException"/> class.
    /// </summary>
    /// <param name="message">Message String</param>
    public EasyMessagesException(string message)
        : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="EasyMessagesException"/> class with an inner exception.
    /// </summary>
    /// <param name="message">Message String</param>
    /// <param name="innerException">Inner Exception</param>
    public EasyMessagesException(string message, Exception innerException)
        : base(message, innerException) { }
}
