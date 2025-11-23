namespace RecurPixel.EasyMessages;

/// <summary>
/// Defines the type of message.
/// </summary>
public enum MessageType
{
    /// <summary>
    /// Success Message
    /// </summary>
    Success = 0,

    /// <summary>
    /// Informational Message
    /// </summary>
    Info = 1,

    /// <summary>
    /// Warning Message
    /// </summary>
    Warning = 2,

    /// <summary>
    /// Error Message
    /// </summary>
    Error = 3,

    /// <summary>
    /// Critical Message
    /// </summary>
    Critical = 4,
}
