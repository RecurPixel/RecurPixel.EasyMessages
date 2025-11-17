using System.Text.Json;
using RecurPixel.EasyMessages.Core;
using RecurPixel.EasyMessages.Formatters;

namespace RecurPixel.EasyMessages.Core.Extensions;

public static class MessageExtensions
{
    public static Message WithData(this Message message, object data)
    {
        return message with { Data = data };
    }

    public static Message WithCorrelationId(this Message message, string correlationId)
    {
        return message with { CorrelationId = correlationId };
    }

    public static Message WithMetadata(this Message message, string key, object value)
    {
        var metadata = new Dictionary<string, object>(message.Metadata ?? new());
        metadata[key] = value;
        return message with { Metadata = metadata };
    }

    public static Message WithStatusCode(this Message message, int statusCode)
    {
        return message with { HttpStatusCode = statusCode };
    }

    public static Message WithParams(this Message message, object parameters)
    {
        var properties = parameters.GetType().GetProperties();
        var title = message.Title;
        var description = message.Description;

        foreach (var prop in properties)
        {
            var value = prop.GetValue(parameters)?.ToString() ?? "{field}";
            var placeholder = $"{{{prop.Name}}}";

            title = title.Replace(placeholder, value, StringComparison.OrdinalIgnoreCase);
            description = description.Replace(placeholder, value);
        }

        return message with
        {
            Title = title,
            Description = description,
        };
    }

    public static string ToJson(this Message message, JsonSerializerOptions? options = null)
    {
        return new JsonFormatter(options).Format(message);
    }

    public static object ToJsonObject(this Message message)
    {
        return new JsonFormatter().FormatAsObject(message);
    }

    public static void ToConsole(this Message message, bool useColors = true)
    {
        new ConsoleFormatter(useColors).WriteToConsole(message);
    }
}
