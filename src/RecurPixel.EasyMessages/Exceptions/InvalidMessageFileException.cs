namespace RecurPixel.EasyMessages.Exceptions;

public class InvalidMessageFileException : EasyMessagesException
{
    public InvalidMessageFileException(string message)
        : base(message) { }

    public InvalidMessageFileException(string message, Exception innerException)
        : base(message, innerException) { }
}
