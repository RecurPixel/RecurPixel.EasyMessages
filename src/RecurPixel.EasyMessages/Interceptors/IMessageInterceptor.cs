using RecurPixel.EasyMessages;

namespace RecurPixel.EasyMessages.Interceptors;

public interface IMessageInterceptor
{
    Message OnBeforeFormat(Message message);
    Message OnAfterFormat(Message message);
}
