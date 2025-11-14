using RecurPixel.EasyMessages.Core;
using RecurPixel.EasyMessages.Core.Extensions;

namespace RecurPixel.EasyMessages.Facades;

public class ValidationMessages
{
    public static Message Failed() => MessageRegistry.Get("VAL_001");
    
    public static Message RequiredField(string field) =>
        MessageRegistry.Get("VAL_002").WithParams(new { field });
    
    public static Message InvalidFormat(string field) =>
        MessageRegistry.Get("VAL_003").WithParams(new { field });
}