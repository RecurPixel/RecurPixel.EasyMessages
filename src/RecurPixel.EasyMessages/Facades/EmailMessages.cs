using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages;

public static partial class Msg
{
    /// <summary>
    /// Email and notification messages.
    /// </summary>
    public static class Email
    {
        /// <summary>Returns EMAIL_001: Your email has been sent to {recipient}.</summary>
        public static Message Sent(string? recipient = null)
        {
            var message = MessageRegistry.Get(MessageCodes.Email.EmailSentSuccessfully);

            return recipient != null ? message.WithParams(new { recipient }) : message;
        }

        /// <summary>Returns EMAIL_002: Failed to send email to {recipient}.</summary>
        public static Message DeliveryFailed(string? recipient = null)
        {
            var message = MessageRegistry.Get(MessageCodes.Email.EmailDeliveryFailed);

            return recipient != null ? message.WithParams(new { recipient }) : message;
        }

        /// <summary>Returns EMAIL_003: Your email address has been verified successfully.</summary>
        public static Message Verified() => MessageRegistry.Get(MessageCodes.Email.EmailVerified);

        /// <summary>Returns EMAIL_004: The email verification link is invalid or has expired.</summary>
        public static Message InvalidVerificationLink() => MessageRegistry.Get(MessageCodes.Email.InvalidVerificationLink);

        /// <summary>Returns EMAIL_005: A verification email has been sent to {email}.</summary>
        public static Message VerificationSent(string? email = null)
        {
            var message = MessageRegistry.Get(MessageCodes.Email.VerificationEmailSent);

            return email != null ? message.WithParams(new { email }) : message;
        }
    }
}
