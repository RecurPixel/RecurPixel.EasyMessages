using RecurPixel.EasyMessages;

namespace RecurPixel.EasyMessages.Interceptors;

/// <summary>
/// Registry for Message Interceptors
/// </summary>
public static class InterceptorRegistry
{
    private static readonly List<IMessageInterceptor> _interceptors = new();
    private static readonly object _lock = new();

    /// <summary>
    /// Registers a message interceptor.
    /// </summary>
    /// <param name="interceptor">IMessageInterceptor Object</param>
    public static void Register(IMessageInterceptor interceptor)
    {
        lock (_lock)
        {
            _interceptors.Add(interceptor);
        }
    }

    /// <summary>
    /// Clears all registered interceptors.
    /// </summary>
    public static void Clear()
    {
        lock (_lock)
        {
            _interceptors.Clear();
        }
    }

    /// <summary>
    /// Invokes the OnBeforeFormat method of all registered interceptors.
    /// </summary>
    /// <param name="message">Message Object</param>
    /// <returns>Message Object</returns>
    internal static Message InvokeBeforeFormat(Message message)
    {
        var result = message;
        lock (_lock)
        {
            foreach (var interceptor in _interceptors)
            {
                result = interceptor.OnBeforeFormat(result);
            }
        }
        return result;
    }

    /// <summary>
    /// Invokes the OnAfterFormat method of all registered interceptors.
    /// </summary>
    /// <param name="message">Message Object</param>
    /// <returns>Message Object</returns>
    internal static Message InvokeAfterFormat(Message message)
    {
        var result = message;
        lock (_lock)
        {
            foreach (var interceptor in _interceptors)
            {
                result = interceptor.OnAfterFormat(result);
            }
        }
        return result;
    }
}
