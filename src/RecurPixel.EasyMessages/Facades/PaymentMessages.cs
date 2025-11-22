using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages;

public static partial class Msg
{
    /// <summary>
    /// Payment and transaction messages.
    /// </summary>
    public static class Payment
    {
        /// <summary>Returns PAY_001: Your payment of {amount} has been processed successfully.</summary>
        public static Message Successful(string? amount = null)
        {
            var message = MessageRegistry.Get(MessageCodes.Pay.PaymentSuccessful);

            return amount != null ? message.WithParams(new { amount }) : message;
        }

        /// <summary>Returns PAY_002: Your payment could not be processed.</summary>
        public static Message Failed() => MessageRegistry.Get(MessageCodes.Pay.PaymentFailed);

        /// <summary>Returns PAY_003: Your account has insufficient funds to complete this transaction.</summary>
        public static Message InsufficientFunds() =>
            MessageRegistry.Get(MessageCodes.Pay.InsufficientFunds);

        /// <summary>Returns PAY_004: Your card was declined by the issuing bank.</summary>
        public static Message CardDeclined() => MessageRegistry.Get(MessageCodes.Pay.CardDeclined);

        /// <summary>Returns PAY_005: The card details provided are invalid.</summary>
        public static Message InvalidCard() =>
            MessageRegistry.Get(MessageCodes.Pay.InvalidCardDetails);

        /// <summary>Returns PAY_006: The card has expired. Please use a different payment method.</summary>
        public static Message CardExpired() => MessageRegistry.Get(MessageCodes.Pay.CardExpired);

        /// <summary>Returns PAY_007: Your refund of {amount} has been processed.</summary>
        public static Message RefundProcessed(string? amount = null)
        {
            var message = MessageRegistry.Get(MessageCodes.Pay.RefundProcessed);

            return amount != null ? message.WithParams(new { amount }) : message;
        }

        /// <summary>Returns PAY_008: Unable to process your refund request.</summary>
        public static Message RefundFailed() => MessageRegistry.Get(MessageCodes.Pay.RefundFailed);

        /// <summary>Returns PAY_009: Your payment is being processed. This may take a few minutes.</summary>
        public static Message Pending() => MessageRegistry.Get(MessageCodes.Pay.PaymentPending);

        /// <summary>Returns PAY_010: This transaction exceeds your daily limit of {limit}.</summary>
        public static Message LimitExceeded(int? limit = null)
        {
            var message = MessageRegistry.Get(MessageCodes.Pay.TransactionLimitExceeded);

            return limit != null ? message.WithParams(new { limit }) : message;
        }

        /// <summary>Returns PAY_011: The payment gateway is temporarily unavailable.</summary>
        public static Message GatewayError() =>
            MessageRegistry.Get(MessageCodes.Pay.PaymentGatewayError);

        /// <summary>Returns PAY_012: Your subscription has been activated successfully.</summary>
        public static Message SubscriptionActivated() =>
            MessageRegistry.Get(MessageCodes.Pay.SubscriptionActivated);

        /// <summary>Returns PAY_013: Your subscription will expire on {expiryDate}.</summary>
        public static Message SubscriptionExpiring(DateTime? expiryDate = null)
        {
            var message = MessageRegistry.Get(MessageCodes.Pay.SubscriptionExpiringSoon);

            return expiryDate != null ? message.WithParams(new { expiryDate }) : message;
        }

        /// <summary>Returns PAY_014: Your subscription has been cancelled.</summary>
        public static Message SubscriptionCancelled() =>
            MessageRegistry.Get(MessageCodes.Pay.SubscriptionCancelled);
    }
}
