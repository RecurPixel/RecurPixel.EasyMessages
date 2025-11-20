using RecurPixel.EasyMessages;

namespace RecurPixel.EasyMessages.Interceptors;

public static class InterceptorRegistry
{
    private static readonly List<IMessageInterceptor> _interceptors = new();
    private static readonly object _lock = new();

    public static void Register(IMessageInterceptor interceptor)
    {
        lock (_lock)
        {
            _interceptors.Add(interceptor);
        }
    }

    public static void Clear()
    {
        lock (_lock)
        {
            _interceptors.Clear();
        }
    }

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
