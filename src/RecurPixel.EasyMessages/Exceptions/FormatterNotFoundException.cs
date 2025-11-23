namespace RecurPixel.EasyMessages.Exceptions;

/// <summary>
/// Exception thrown when a formatter is not found.
/// </summary>
public class FormatterNotFoundException : EasyMessagesException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FormatterNotFoundException"/> class.
    /// </summary>
    /// <param name="message">Message String</param>
    public FormatterNotFoundException(string message)
        : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="FormatterNotFoundException"/> class with an inner exception.
    /// </summary>
    /// <param name="message">Message String</param>
    /// <param name="innerException">Inner Exeption</param>
    public FormatterNotFoundException(string message, Exception innerException)
        : base(message, innerException) { }
}
