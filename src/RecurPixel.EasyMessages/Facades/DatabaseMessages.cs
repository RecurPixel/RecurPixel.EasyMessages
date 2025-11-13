using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages.Facades;

public static class DatabaseMessages
{
    public static Message ConnectionFailed() => MessageRegistry.Get("DB_001");
}