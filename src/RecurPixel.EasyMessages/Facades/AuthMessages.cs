using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages;

public static partial class Msg
{
    public static class Auth
    {
        // Assuming Message is a defined type and MessageRegistry is a static helper.
        public static Message LoginFailed() =>
            MessageRegistry.Get(MessageCodes.AuthenticationFailed);

        public static Message Unauthorized() =>
            MessageRegistry.Get(MessageCodes.UnauthorizedAccess);

        public static Message LoginSuccess() => MessageRegistry.Get(MessageCodes.LoginSuccessful);

        // Example of another method
        public static Message PasswordResetRequired() =>
            MessageRegistry.Get(MessageCodes.PasswordResetRequired);

        public static Message Forbidden() => Unauthorized(); // Alias
    }
}
