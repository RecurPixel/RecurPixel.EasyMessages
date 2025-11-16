using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages.Outputs;

public interface IMessageOutput
{
    Task SendAsync(Message message);
}
