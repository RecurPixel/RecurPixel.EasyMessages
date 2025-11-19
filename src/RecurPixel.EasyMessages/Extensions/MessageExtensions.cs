using System.Text.Json;
using System.Xml.Linq;
using RecurPixel.EasyMessages.Core;
using RecurPixel.EasyMessages.Exceptions;
using RecurPixel.EasyMessages.Formatters;
using RecurPixel.EasyMessages.Helpers;

namespace RecurPixel.EasyMessages;

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

    // TODO: Make field check case insensitive.
    public static Message WithParams(this Message message, object parameters)
    {
        if (!TypeExtensions.IsAnonymousType(parameters.GetType()))
        {
            throw new InvalidMessageParameterFileException("Invalid Parameter Type.");
        }

        var paramDict = parameters
            .GetType()
            .GetProperties()
            .ToDictionary(p => p.Name, p => p.GetValue(parameters));

        var title = message.Title;
        var description = message.Description;

        foreach (var (key, value) in paramDict)
        {
            var placeholder = $"{{{key}}}";
            var replacement = value?.ToString() ?? "";

            title = title.Replace(placeholder, replacement);
            description = description.Replace(placeholder, replacement);
        }

        return message with
        {
            Title = title,
            Description = description,
            Parameters = paramDict,
        };
    }

    public static string ToJson(
        this Message message,
        FormatterOptions? options = null,
        JsonSerializerOptions? jsonOptions = null
    )
    {
        return new JsonFormatter(options, jsonOptions).Format(message);
    }

    public static object ToJsonObject(
        this Message message,
        FormatterOptions? options = null,
        JsonSerializerOptions? jsonOptions = null
    )
    {
        return new JsonFormatter(options, jsonOptions).FormatAsObject(message);
    }

    public static void ToConsole(this Message message, bool useColors = true)
    {
        new ConsoleFormatter(useColors).WriteToConsole(message);
    }

    /// <summary>
    /// Converts message to plain text
    /// </summary>
    public static string ToPlainText(this Message message, FormatterOptions? options = null)
    {
        return new PlainTextFormatter(options).Format(message);
    }

    /// <summary>
    /// Converts message to XML string
    /// </summary>
    public static string ToXml(this Message message, FormatterOptions? options = null)
    {
        return new XmlFormatter(options).Format(message);
    }

    /// <summary>
    /// Converts message to XML document
    /// </summary>
    public static XDocument ToXmlDocument(this Message message, FormatterOptions? options = null)
    {
        return (XDocument)new XmlFormatter(options).FormatAsObject(message);
    }
}
