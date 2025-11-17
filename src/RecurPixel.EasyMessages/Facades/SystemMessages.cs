using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages;

public static partial class Msg
{
    public static class System
    {
        public static Message Error() => MessageRegistry.Get("SYS_001");

        public static Message Processing() => MessageRegistry.Get("SYS_002");
    }
}
