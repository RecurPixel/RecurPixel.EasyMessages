using RecurPixel.EasyMessages.Core;
using RecurPixel.EasyMessages.Core.Extensions;


namespace RecurPixel.EasyMessages.Facades;

public class FileMessages
{
    public static Message Uploaded() => MessageRegistry.Get("FILE_001");
    
    public static Message InvalidType(params string[] types) =>
        MessageRegistry.Get("FILE_002")
            .WithParams(new { types = string.Join(", ", types) });
}