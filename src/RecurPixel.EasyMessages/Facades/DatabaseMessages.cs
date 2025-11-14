using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages.Facades;

public class DatabaseMessages
{
    public static Message ConnectionFailed() => MessageRegistry.Get("DB_001");
}