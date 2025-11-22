using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages;

public static partial class Msg
{
    /// <summary>
    /// System operation messages.
    /// </summary>
    public static class System
    {
        /// <summary>Returns SYS_001: An unexpected error occurred. Please try again later.</summary>
        public static Message Error() => MessageRegistry.Get(MessageCodes.Sys.SystemError);

        /// <summary>Returns SYS_002: Your request is being processed. Please wait...</summary>
        public static Message Processing() =>
            MessageRegistry.Get(MessageCodes.Sys.ProcessingRequest);

        /// <summary>Returns SYS_003: Some features may be temporarily unavailable.</summary>
        public static Message Degraded() => MessageRegistry.Get(MessageCodes.Sys.ServiceDegraded);

        /// <summary>Returns SYS_004: The system is currently under maintenance.</summary>
        public static Message Maintenance() =>
            MessageRegistry.Get(MessageCodes.Sys.MaintenanceMode);

        /// <summary>Returns SYS_005: The operation completed successfully.</summary>
        public static Message OperationCompleted() =>
            MessageRegistry.Get(MessageCodes.Sys.OperationCompleted);

        /// <summary>Returns SYS_006: You have exceeded the rate limit.</summary>
        public static Message RateLimitExceeded() =>
            MessageRegistry.Get(MessageCodes.Sys.RateLimitExceeded);

        /// <summary>Returns SYS_007: The service is temporarily unavailable.</summary>
        public static Message Unavailable() =>
            MessageRegistry.Get(MessageCodes.Sys.ServiceUnavailable);

        /// <summary>Returns SYS_008: Your request has been queued for processing.</summary>
        public static Message Queued() => MessageRegistry.Get(MessageCodes.Sys.RequestQueued);

        /// <summary>Returns SYS_009: The request timed out. Please try again.</summary>
        public static Message Timeout() => MessageRegistry.Get(MessageCodes.Sys.Timeout);

        /// <summary>Returns SYS_010: A system configuration error occurred.</summary>
        public static Message ConfigurationError() =>
            MessageRegistry.Get(MessageCodes.Sys.ConfigurationError);
    }
}
