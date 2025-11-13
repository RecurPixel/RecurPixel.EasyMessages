using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages.Facades;

public static class CustomMessages
{
    public static Message Get(string code) => MessageRegistry.Get(code);
}
