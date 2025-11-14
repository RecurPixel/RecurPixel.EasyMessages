using RecurPixel.EasyMessages.Core;


namespace RecurPixel.EasyMessages.Facades;

public class AuthMessages
{
    // Assuming Message is a defined type and MessageRegistry is a static helper.
    public Message LoginFailed() => MessageRegistry.Get(MessageCodes.AuthenticationFailed);
    public Message Unauthorized() => MessageRegistry.Get(MessageCodes.UnauthorizedAccess);
    public Message LoginSuccess() => MessageRegistry.Get(MessageCodes.LoginSuccessful);
    
    // Example of another method
    public Message PasswordResetRequired() => MessageRegistry.Get(MessageCodes.PasswordResetRequired);
}