using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages;

public static partial class Msg
{
    public static class Database
    {
        public static Message ConnectionFailed() => MessageRegistry.Get("DB_001");
    }
}
