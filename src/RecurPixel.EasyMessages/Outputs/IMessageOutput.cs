// Planned for future Versions.

using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages.Outputs;

public interface IMessageOutput
{
    Task SendAsync(Message message);
}


// // Examples:
// - EmailOutput.SendAsync() → sends email
// - SlackOutput.SendAsync() → posts to Slack
// - SmsOutput.SendAsync() → sends SMS