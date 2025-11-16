using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages.Interceptors;

public interface IMessageInterceptor
{
    Message OnBeforeFormat(Message message);
    Message OnAfterFormat(Message message);
}
