using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages;

public static partial class Msg
{
    public static Message Custom(string code) => MessageRegistry.Get(code);
}
