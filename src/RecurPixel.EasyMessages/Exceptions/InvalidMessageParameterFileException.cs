namespace RecurPixel.EasyMessages.Exceptions;

/// <summary>
/// Exception thrown when a message parameter file is invalid.
/// </summary>
public class InvalidMessageParameterFileException : EasyMessagesException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidMessageParameterFileException"/> class.
    /// </summary>
    /// <param name="message">Message String</param>
    public InvalidMessageParameterFileException(string message)
        : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidMessageParameterFileException"/> class with an inner exception.
    /// </summary>
    /// <param name="message">Message String</param>
    /// <param name="innerException">Inner Exception</param>
    public InvalidMessageParameterFileException(string message, Exception innerException)
        : base(message, innerException) { }
}
