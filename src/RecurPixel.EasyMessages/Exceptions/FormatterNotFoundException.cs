namespace RecurPixel.EasyMessages.Exceptions;

public class FormatterNotFoundException : EasyMessagesException
{
    public FormatterNotFoundException(string message)
        : base(message) { }

    public FormatterNotFoundException(string message, Exception innerException)
        : base(message, innerException) { }
}
