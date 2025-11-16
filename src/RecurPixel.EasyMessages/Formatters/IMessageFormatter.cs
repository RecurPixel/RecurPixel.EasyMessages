using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages.Formatters;

public interface IMessageFormatter
{
    string Format(Message message);
    object FormatAsObject(Message message);
}