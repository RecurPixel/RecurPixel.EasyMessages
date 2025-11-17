using RecurPixel.EasyMessages.Core;
using RecurPixel.EasyMessages.Core.Extensions;

namespace RecurPixel.EasyMessages;

public static partial class Msg
{
    public static class Validation
    {
        public static Message Failed() => MessageRegistry.Get("VAL_001");

        public static Message RequiredField(string field) =>
            MessageRegistry.Get("VAL_002").WithParams(new { field });

        public static Message InvalidFormat(string field) =>
            MessageRegistry.Get("VAL_003").WithParams(new { field });
    }
}
