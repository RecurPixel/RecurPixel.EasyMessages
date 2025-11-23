using RecurPixel.EasyMessages;

namespace RecurPixel.EasyMessages.Formatters;

/// <summary>
/// Defines the interface for message formatters.
/// </summary>
public interface IMessageFormatter
{
    /// <summary>
    /// Formats the specified message.
    /// </summary>
    /// <param name="message">Message Object</param>
    /// <returns>Formated String</returns>
    string Format(Message message);

    /// <summary>
    /// Formats the specified message as an object.
    /// </summary>
    /// <param name="message">Message Object</param>
    /// <returns>Formeted Object</returns>
    object FormatAsObject(Message message);
}