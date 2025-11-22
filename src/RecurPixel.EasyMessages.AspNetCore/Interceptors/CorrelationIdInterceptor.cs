using Microsoft.AspNetCore.Http;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.Core;
using RecurPixel.EasyMessages.Interceptors;

public class CorrelationIdInterceptor : IMessageInterceptor
{
    private readonly Func<IHttpContextAccessor> _httpContextAccessorFactory;
    private IHttpContextAccessor _httpContextAccessor;

    // Constructor accepts factory function
    public CorrelationIdInterceptor(Func<IHttpContextAccessor> httpContextAccessorFactory)
    {
        _httpContextAccessorFactory = httpContextAccessorFactory;
    }

    // Lazy initialization
    private IHttpContextAccessor HttpContextAccessor =>
        _httpContextAccessor ??= _httpContextAccessorFactory();

    public Message OnBeforeFormat(Message message)
    {
        // Only add correlation ID if not already set
        if (string.IsNullOrEmpty(message.CorrelationId))
        {
            var correlationId = HttpContextAccessor.HttpContext?.TraceIdentifier;
            if (!string.IsNullOrEmpty(correlationId))
            {
                return message with { CorrelationId = correlationId };
            }
        }

        return message;
    }

    public Message OnAfterFormat(Message message) => message;
}
