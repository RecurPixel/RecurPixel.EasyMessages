using System.Text.Json;
using System.Text.Json.Serialization;
using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages.Formatters;

public class JsonFormatter : IMessageFormatter
{
    private readonly JsonSerializerOptions _options;

    public JsonFormatter(JsonSerializerOptions? options = null)
    {
        _options =
            options
            ?? new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };
    }

    public string Format(Message message)
    {
        return JsonSerializer.Serialize(FormatAsObject(message), _options);
    }

    public object FormatAsObject(Message message)
    {
        return new
        {
            success = message.Type is MessageType.Success or MessageType.Info,
            code = message.Code,
            type = message.Type.ToString().ToLowerInvariant(),
            title = message.Title,
            description = message.Description,
            data = message.Data,
            timestamp = message.Timestamp,
            correlationId = message.CorrelationId,
            metadata = message.Metadata,
        };
    }
}
