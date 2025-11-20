using System.Text.Json;
using System.Xml.Linq;
using RecurPixel.EasyMessages.Formatters;

namespace RecurPixel.EasyMessages;

public static partial class MessageExtensions
{
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
