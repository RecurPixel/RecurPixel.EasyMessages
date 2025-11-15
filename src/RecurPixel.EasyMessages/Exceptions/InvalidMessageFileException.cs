namespace RecurPixel.EasyMessages.Exceptions;

public class InvalidMessageFileException : Exception
{
    public InvalidMessageFileException(string message) : base(message)
    {
    }

    public InvalidMessageFileException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}