using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages;

public static partial class Msg
{
    /// <summary>
    /// Authentication and authorization messages.
    /// </summary>
    public static class Auth
    {
        // Assuming Message is a defined type and MessageRegistry is a static helper.
        /// <summary>Returns AUTH_001: Invalid username or password.</summary>
        public static Message LoginFailed() => MessageRegistry.Get(MessageCodes.Auth.AuthenticationFailed);

        /// <summary>Returns AUTH_002: You don't have permission to access this resource.</summary>
        public static Message Unauthorized() => MessageRegistry.Get(MessageCodes.Auth.UnauthorizedAccess);

        /// <summary>Returns AUTH_003: Welcome back!</summary>
        public static Message LoginSuccessful() => MessageRegistry.Get(MessageCodes.Auth.LoginSuccessful);

        /// <summary>Returns AUTH_004: Your session has expired. Please log in again.</summary>
        public static Message SessionExpired() => MessageRegistry.Get(MessageCodes.Auth.SessionExpired);

        /// <summary>Returns AUTH_005: The authentication token is invalid or has expired.</summary>
        public static Message InvalidToken() => MessageRegistry.Get(MessageCodes.Auth.InvalidToken);

        /// <summary>Returns AUTH_006: Your account has been locked due to multiple failed login attempts.</summary>
        public static Message AccountLocked() => MessageRegistry.Get(MessageCodes.Auth.AccountLocked);

        /// <summary>Returns AUTH_007: You have been successfully logged out.</summary>
        public static Message LogoutSuccessful() => MessageRegistry.Get(MessageCodes.Auth.LogoutSuccessful);

        /// <summary>Returns AUTH_008: You must reset your password before continuing.</summary>
        public static Message PasswordResetRequired() => MessageRegistry.Get(MessageCodes.Auth.PasswordResetRequired);

        /// <summary>Returns AUTH_009: Unable to refresh your session. Please log in again.</summary>
        public static Message InvalidRefreshToken() => MessageRegistry.Get(MessageCodes.Auth.InvalidRefreshToken);

        /// <summary>Returns AUTH_010: Please complete multi-factor authentication to continue.</summary>
        public static Message MfaRequired() => MessageRegistry.Get(MessageCodes.Auth.MfaRequired);

        public static Message Forbidden() => Unauthorized(); // Alias
    }
}
