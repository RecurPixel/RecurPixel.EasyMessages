using RecurPixel.EasyMessages;

namespace RecurPixel.EasyMessages.Interceptors;

/// <summary>
/// Message Interceptor Interface
/// </summary>
public interface IMessageInterceptor
{
    /// <summary>
    /// Called before formatting the message
    /// </summary>
    /// <param name="message">Message Object</param>
    /// <returns>Message Object</returns>
    Message OnBeforeFormat(Message message);
    /// <summary>
    /// Called after formatting the message
    /// </summary>
    /// <param name="message">Message Object</param>
    /// <returns>Message Object</returns>
    Message OnAfterFormat(Message message);
}
