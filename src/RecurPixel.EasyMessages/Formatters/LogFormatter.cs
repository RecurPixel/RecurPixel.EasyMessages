using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages.Formatters;

public class LogFormatter : IMessageFormatter
{
    /// <summary>
    /// Formats the message as a simple, human-readable string.
    /// Example output: [2025-11-16 10:47:19.000] [INFO] This is the log content. (User=Jane)
    /// </summary>
    /// <param name="message">The log message to format.</param>
    /// <returns>A formatted log string.</returns>
    public string Format(Message message)
    {
        // Format the timestamp
        string timestampStr = message.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff");

        // Build the basic string part
        string formattedString = $"[{timestampStr}] [{message.Type}] {message.Title}";

        return formattedString;
    }

    /// <summary>
    /// Formats the message as an object suitable for structured logging (e.g., JSON output).
    /// </summary>
    /// <param name="message">The log message to format.</param>
    /// <returns>An object containing structured log data.</returns>
    public object FormatAsObject(Message message)
    {
        // Using an anonymous type is a common way to return a structured object.
        // For more complex/dynamic scenarios, you might use a Dictionary<string, object> or a dedicated DTO.
        var logObject = new Dictionary<string, object>
        {
            { "Timestamp", message.Timestamp },
            { "Type", message.Type },
            { "Message", message.Title },
            { "Description", message.Description }
        };

        return logObject;
    }
}