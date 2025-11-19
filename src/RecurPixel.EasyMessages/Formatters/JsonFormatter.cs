using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages.Formatters;

public class JsonFormatter : IMessageFormatter
{
    private readonly FormatterOptions _options;
    private readonly JsonSerializerOptions _jsonOptions;

    public JsonFormatter(
        FormatterOptions? options = null,
        JsonSerializerOptions? jsonOptions = null
    )
    {
        _options = options ?? FormatterOptions.Default;
        _jsonOptions =
            jsonOptions
            ?? new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };
    }

    public string Format(Message message)
    {
        return JsonSerializer.Serialize(FormatAsObject(message), _jsonOptions);
    }

    public object FormatAsObject(Message message)
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
            ((Dictionary<string, object?>)result["message"])["hint"] = message.Hint;

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
                    metadata[key] = value;
            }

            if (metadata.Count > 0)
                result["metadata"] = metadata;
        }

        return result;
    }
}
