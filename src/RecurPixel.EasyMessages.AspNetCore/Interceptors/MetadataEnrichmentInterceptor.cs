using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.Interceptors;
using Microsoft.AspNetCore.Http;

public class MetadataEnrichmentInterceptor : IMessageInterceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public MetadataEnrichmentInterceptor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public Message OnBeforeFormat(Message message)
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null) return message;
        
        var metadata = new Dictionary<string, object>(message.Metadata ?? new())
        {
            ["RequestPath"] = context.Request.Path.Value ?? "",
            ["RequestMethod"] = context.Request.Method,
            ["UserAgent"] = context.Request.Headers["User-Agent"].ToString()
        };
        
        return message with { Metadata = metadata };
    }
    
    public Message OnAfterFormat(Message message) => message;
}