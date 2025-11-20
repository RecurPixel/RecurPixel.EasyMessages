using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.Interceptors;

namespace RecurPixel.EasyMessages.Formatters;

public abstract class MessageFormatterBase : IMessageFormatter
{
    public string Format(Message message)
    {
        // Invoke interceptors
        var beforeMessage = InterceptorRegistry.InvokeBeforeFormat(message);
        var formatted = FormatCore(beforeMessage);
        var afterMessage = InterceptorRegistry.InvokeAfterFormat(beforeMessage);

        return formatted;
    }

    protected abstract string FormatCore(Message message);

    public abstract object FormatAsObject(Message message);
}
