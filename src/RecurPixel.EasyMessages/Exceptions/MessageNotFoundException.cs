namespace RecurPixel.EasyMessages.Exceptions;

public class MessageNotFoundException : EasyMessagesException
{
    public MessageNotFoundException(string message)
        : base(message) { }

    public MessageNotFoundException(string message, Exception innerException)
        : base(message, innerException) { }
}
