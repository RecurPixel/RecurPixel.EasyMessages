using RecurPixel.EasyMessages.Interceptors;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.Core;
using Microsoft.AspNetCore.Http;

public class CorrelationIdInterceptor : IMessageInterceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public CorrelationIdInterceptor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public Message OnBeforeFormat(Message message)
    {
        if (string.IsNullOrEmpty(message.CorrelationId))
        {
            var correlationId = _httpContextAccessor.HttpContext?.TraceIdentifier;
            return message with { CorrelationId = correlationId };
        }
        return message;
    }
    
    public Message OnAfterFormat(Message message) => message;
}