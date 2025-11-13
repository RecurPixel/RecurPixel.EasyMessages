using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages.Facades;

public static class SystemMessages
{
    public static Message Error() => MessageRegistry.Get("SYS_001");
    public static Message Processing() => MessageRegistry.Get("SYS_002");
}