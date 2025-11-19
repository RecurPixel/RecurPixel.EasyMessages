namespace RecurPixel.EasyMessages.Exceptions;

public class InvalidMessageParameterFileException : EasyMessagesException
{
    public InvalidMessageParameterFileException(string message)
        : base(message) { }

    public InvalidMessageParameterFileException(string message, Exception innerException)
        : base(message, innerException) { }
}
