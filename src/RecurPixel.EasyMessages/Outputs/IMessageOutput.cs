// Planned for future Versions.

using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages.Outputs;

/// <summary>
/// Defines the interface for message output mechanisms.
/// </summary>
public interface IMessageOutput
{
    /// <summary>
    /// Sends the specified message asynchronously.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    Task SendAsync(Message message);
}


// // Examples:
// - EmailOutput.SendAsync() → sends email
// - SlackOutput.SendAsync() → posts to Slack
// - SmsOutput.SendAsync() → sends SMS