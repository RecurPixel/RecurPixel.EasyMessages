using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.Interceptors;

namespace RecurPixel.EasyMessages.Formatters;

/// <summary>
/// Base class for formatters that support interceptors.
/// Extend this class to automatically invoke registered interceptors
/// before and after formatting.
/// </summary>
public abstract class MessageFormatterBase : IMessageFormatter
{
    public string Format(Message message)
    {
        // Invoke interceptors before formatting
        var beforeMessage = InterceptorRegistry.InvokeBeforeFormat(message);

        // Subclass implements actual formatting
        var formatted = FormatCore(beforeMessage);

        // Invoke interceptors after formatting
        var afterMessage = InterceptorRegistry.InvokeAfterFormat(beforeMessage);

        return formatted;
    }


    /// <summary>
    /// Override this to implement formatting logic
    /// </summary>
    protected abstract string FormatCore(Message message);

    /// <summary>
    /// Override this to implement object formatting logic
    /// </summary>
    public abstract object FormatAsObject(Message message);
}
