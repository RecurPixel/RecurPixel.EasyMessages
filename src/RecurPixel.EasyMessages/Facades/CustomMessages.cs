using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages;

/// <summary>
/// Facade for custom messages.
/// </summary>
public static partial class Msg
{
    /// <summary>
    /// Returns a custom message by code.
    /// </summary>
    /// <param name="code">Custom Message Code</param>
    /// <returns>Message Object</returns>
    public static Message Custom(string code) => MessageRegistry.Get(code);
}
