using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages.Interceptors;

public interface IMessageLoader
{
    internal Task<Dictionary<string, MessageTemplate>> LoadAsync();
}
