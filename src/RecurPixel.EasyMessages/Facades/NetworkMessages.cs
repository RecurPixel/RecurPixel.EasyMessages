using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages;

public static partial class Msg
{
    /// <summary>
    /// Network and API messages.
    /// </summary>
    public static class Network
    {
        /// <summary>Returns NET_001: A network error occurred. Please check your connection.</summary>
        public static Message Error() => MessageRegistry.Get(MessageCodes.Net.NetworkError);

        /// <summary>Returns NET_002: The request timed out. Please try again.</summary>
        public static Message Timeout() => MessageRegistry.Get(MessageCodes.Net.RequestTimeout);

        /// <summary>Returns NET_003: The server could not understand your request.</summary>
        public static Message BadRequest() => MessageRegistry.Get(MessageCodes.Net.BadRequest);

        /// <summary>Returns NET_004: The server encountered an error. Please try again later.</summary>
        public static Message ServerError() => MessageRegistry.Get(MessageCodes.Net.ServerError);

        /// <summary>Returns NET_005: You have exceeded the API rate limit.</summary>
        public static Message RateLimitExceeded() =>
            MessageRegistry.Get(MessageCodes.Net.ApiRateLimitExceeded);

        /// <summary>Returns NET_006: Unable to connect to the server.</summary>
        public static Message ConnectionRefused() =>
            MessageRegistry.Get(MessageCodes.Net.ConnectionRefused);

        /// <summary>Returns NET_007: The server's SSL certificate is invalid or expired.</summary>
        public static Message SslError() =>
            MessageRegistry.Get(MessageCodes.Net.SslCertificateError);

        /// <summary>Returns NET_008: Your connection is slower than usual. This may affect performance.</summary>
        public static Message SlowConnection() =>
            MessageRegistry.Get(MessageCodes.Net.SlowConnection);

        /// <summary>Returns NET_009: The gateway did not receive a timely response from the upstream server.</summary>
        public static Message GatewayTimeout() =>
            MessageRegistry.Get(MessageCodes.Net.GatewayTimeout);

        /// <summary>Returns NET_010: Successfully connected to {service}.</summary>
        public static Message Connected(string? service = null)
        {
            var message = MessageRegistry.Get(MessageCodes.Net.ConnectionEstablished);

            return service != null ? message.WithParams(new { service }) : message;
        }
    }
}
