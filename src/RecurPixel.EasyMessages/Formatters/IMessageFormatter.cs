using RecurPixel.EasyMessages;

namespace RecurPixel.EasyMessages.Formatters;

public interface IMessageFormatter
{
    string Format(Message message);
    object FormatAsObject(Message message);
}