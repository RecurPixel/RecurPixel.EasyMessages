namespace RecurPixel.EasyMessages.Exceptions;

public class EasyMessagesException : Exception
{
    public EasyMessagesException(string message)
        : base(message) { }

    public EasyMessagesException(string message, Exception innerException)
        : base(message, innerException) { }
}
