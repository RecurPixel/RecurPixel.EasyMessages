using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.AspNetCore.Configuration;
using RecurPixel.EasyMessages.Interceptors;

public class MetadataEnrichmentInterceptor : IMessageInterceptor
{
    private readonly Func<IHttpContextAccessor> _httpContextAccessorFactory;
    private readonly MetadataEnrichmentFields _fields;
    private IHttpContextAccessor? _httpContextAccessor;

    public MetadataEnrichmentInterceptor(
        Func<IHttpContextAccessor> httpContextAccessorFactory,
        MetadataEnrichmentFields fields
    )
    {
        _httpContextAccessorFactory = httpContextAccessorFactory;
        _fields = fields;
    }

    private IHttpContextAccessor HttpContextAccessor =>
        _httpContextAccessor ??= _httpContextAccessorFactory();

    public Message OnBeforeFormat(Message message)
    {
        var context = HttpContextAccessor.HttpContext;
        if (context == null)
            return message;

        var metadata = new Dictionary<string, object>(message.Metadata ?? new());

        if (_fields.IncludeRequestPath)
            metadata["RequestPath"] = context.Request.Path.Value ?? "";

        if (_fields.IncludeRequestMethod)
            metadata["RequestMethod"] = context.Request.Method;

        if (_fields.IncludeUserAgent)
            metadata["UserAgent"] = context.Request.Headers["User-Agent"].ToString();

        if (_fields.IncludeIpAddress)
            metadata["IpAddress"] = context.Connection.RemoteIpAddress?.ToString() ?? "";

        if (_fields.IncludeUserId && context.User.Identity?.IsAuthenticated == true)
            metadata["UserId"] = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

        if (_fields.IncludeUserName && context.User.Identity?.IsAuthenticated == true)
            metadata["UserName"] = context.User.Identity.Name ?? "";

        return message with
        {
            Metadata = metadata,
        };
    }

    public Message OnAfterFormat(Message message) => message;
}
