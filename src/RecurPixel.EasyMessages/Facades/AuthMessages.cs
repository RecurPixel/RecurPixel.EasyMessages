using RecurPixel.EasyMessages.Core;


namespace RecurPixel.EasyMessages.Facades;

public class AuthMessages
{
    // Assuming Message is a defined type and MessageRegistry is a static helper.
    public Message LoginFailed() => MessageRegistry.Get("AUTH_001");
    public Message Unauthorized() => MessageRegistry.Get("AUTH_002");
    public Message LoginSuccess() => MessageRegistry.Get("AUTH_003");
    
    // Example of another method
    public Message PasswordResetRequired() => MessageRegistry.Get("AUTH_004");
}