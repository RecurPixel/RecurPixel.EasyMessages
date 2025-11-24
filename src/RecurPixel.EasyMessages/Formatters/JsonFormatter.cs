using System.Text.Json;
using System.Text.Json.Serialization;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.Configuration;

namespace RecurPixel.EasyMessages.Formatters;

/// <summary>
/// Formats messages as JSON strings
/// </summary>
public class JsonFormatter : MessageFormatterBase
{
    /// <summary>
    /// Formatter options
    /// </summary>
    private readonly FormatterOptions _options;
    /// <summary>
    /// JSON serialization options
    /// </summary>
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Creates a JSON message formatter
    /// </summary>
    /// <param name="options">FormatterOptions Object</param>
    /// <param name="jsonOptions">JsonSerializerOptions Object</param>
    public JsonFormatter(
        FormatterOptions? options = null,
        JsonSerializerOptions? jsonOptions = null
    )
    {
        _options = options ?? FormatterConfiguration.DefaultOptions;
        _jsonOptions =
            jsonOptions
            ?? new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };
    }

    /// <summary>
    /// Formats the message as a JSON string.
    /// </summary>
    /// <param name="message">Message Object</param>
    /// <returns>Formated Message String</returns>
    protected override string FormatCore(Message message)
    {
        return JsonSerializer.Serialize(FormatAsObject(message), _jsonOptions);
    }

    /// <summary>
    /// Formats the message as an object for JSON serialization.
    /// </summary>
    /// <param name="message">Message Object</param>
    /// <returns>Formated Message Object</returns>
    public override object FormatAsObject(Message message)
    {
        var result = new Dictionary<string, object?>
        {
            ["success"] = message.Type is MessageType.Success or MessageType.Info,
            ["code"] = message.Code,
            ["type"] = message.Type.ToString().ToLowerInvariant(),
        };

        // Timestamp
        if (_options.IncludeTimestamp)
            result["timestamp"] = message.Timestamp;

        // Correlation ID
        if (_options.IncludeCorrelationId && !string.IsNullOrEmpty(message.CorrelationId))
            result["correlationId"] = message.CorrelationId;

        // Message content
        result["message"] = new Dictionary<string, object?>
        {
            ["title"] = message.Title,
            ["description"] = message.Description,
        };
        
        if (_options.IncludeHint && !string.IsNullOrEmpty(message.Hint))
        {
            var msgValue = result["message"];

            if (msgValue is Dictionary<string, object?> messageDict)
            {
                messageDict["hint"] = message.Hint;
                result["message"] = messageDict;
            }
        }

        // Data
        if (_options.IncludeData && message.Data != null)
            result["data"] = message.Data;

        // Parameters
        if (_options.IncludeParameters && message.Parameters?.Count > 0)
            result["parameters"] = message.Parameters;

        // Metadata
        if (_options.IncludeMetadata)
        {
            var metadata = new Dictionary<string, object?>();

            if (_options.IncludeHttpStatusCode)
                metadata["httpStatusCode"] = message.HttpStatusCode;

            if (message.Metadata?.Count > 0)
            {
                foreach (var (key, value) in message.Metadata)
                {
                    metadata[key] = value;
                }
            }

            if (metadata.Count > 0)
                result["metadata"] = metadata;
        }

        return result;
    }
}
