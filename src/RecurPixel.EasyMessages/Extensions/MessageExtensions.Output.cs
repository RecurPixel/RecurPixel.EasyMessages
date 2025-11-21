using System.Text.Json;
using System.Xml.Linq;
using RecurPixel.EasyMessages.Formatters;

namespace RecurPixel.EasyMessages;

public static partial class MessageExtensions
{
    /// <summary>
    /// Format message using a registered formatter by type
    /// </summary>
    public static string ToFormat<TFormatter>(this Message message)
        where TFormatter : IMessageFormatter, new()
    {
        return FormatterRegistry.Get<TFormatter>().Format(message);
    }

    /// <summary>
    /// Format message using a registered formatter by name
    /// </summary>
    public static string ToFormat(this Message message, string formatterName)
    {
        return FormatterRegistry.Get(formatterName).Format(message);
    }

    /// <summary>
    /// Format message as object using a registered formatter by name
    /// </summary>
    public static object ToFormatObject(this Message message, string formatterName)
    {
        return FormatterRegistry.Get(formatterName).FormatAsObject(message);
    }

    /// <summary>
    /// Format message as object using a registered formatter by type
    /// </summary>
    public static object ToFormatObject<TFormatter>(this Message message)
        where TFormatter : IMessageFormatter, new()
    {
        return FormatterRegistry.Get<TFormatter>().FormatAsObject(message);
    }

    // ============================================
    // Convenience methods (use registry internally)
    // ============================================

    /// <summary>
    /// Converts message to JSON string
    /// </summary>
    public static string ToJson(
        this Message message,
        FormatterOptions? options = null,
        JsonSerializerOptions? jsonOptions = null
    )
    {
        // If custom options provided, create new instance
        if (options != null || jsonOptions != null)
        {
            return new JsonFormatter(options, jsonOptions).Format(message);
        }

        // Otherwise use registered formatter
        return message.ToFormat("json");
    }

    /// <summary>
    /// Converts message to JSON object
    /// </summary>
    public static object ToJsonObject(
        this Message message,
        FormatterOptions? options = null,
        JsonSerializerOptions? jsonOptions = null
    )
    {
        // If custom options provided, create new instance
        if (options != null || jsonOptions != null)
        {
            return new JsonFormatter(options, jsonOptions).FormatAsObject(message);
        }

        // Otherwise use registered formatter
        return message.ToFormatObject("json");
    }

    /// <summary>
    /// Converts message to console output with optional colors
    /// </summary>
    public static void ToConsole(this Message message, bool useColors = true)
    {
        new ConsoleFormatter(useColors).WriteToConsole(message);
    }

    /// <summary>
    /// Converts message to plain text
    /// </summary>
    public static string ToPlainText(this Message message, FormatterOptions? options = null)
    {
        if (options != null)
        {
            return new PlainTextFormatter(options).Format(message);
        }

        return message.ToFormat("text");
    }

    /// <summary>
    /// Converts message to XML string
    /// </summary>
    public static string ToXml(this Message message, FormatterOptions? options = null)
    {
        if (options != null)
        {
            return new XmlFormatter(options).Format(message);
        }

        return message.ToFormat("xml");
    }

    /// <summary>
    /// Converts message to XML document
    /// </summary>
    public static XDocument ToXmlDocument(this Message message, FormatterOptions? options = null)
    {
        if (options != null)
        {
            return (XDocument)new XmlFormatter(options).FormatAsObject(message);
        }

        return (XDocument)message.ToFormatObject("xml");
    }
}
