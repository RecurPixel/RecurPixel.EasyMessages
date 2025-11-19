using System.Xml.Linq;
using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages.Formatters;

/// <summary>
/// Formats messages as XML
/// </summary>
public class XmlFormatter : IMessageFormatter
{
    private readonly FormatterOptions _options;

    public XmlFormatter(FormatterOptions? options = null)
    {
        _options = options ?? FormatterOptions.Default;
    }

    public string Format(Message message)
    {
        var root = new XElement(
            "message",
            new XAttribute("code", message.Code),
            new XAttribute("type", message.Type.ToString().ToLowerInvariant()),
            new XAttribute("success", message.Type is MessageType.Success or MessageType.Info)
        );

        if (_options.IncludeTimestamp)
            root.Add(new XAttribute("timestamp", message.Timestamp.ToString("O")));

        if (_options.IncludeCorrelationId && !string.IsNullOrEmpty(message.CorrelationId))
            root.Add(new XAttribute("correlationId", message.CorrelationId));

        // Content
        root.Add(new XElement("title", message.Title));
        root.Add(new XElement("description", message.Description));

        if (_options.IncludeHint && !string.IsNullOrEmpty(message.Hint))
            root.Add(new XElement("hint", message.Hint));

        // Parameters
        if (_options.IncludeParameters && message.Parameters?.Count > 0)
        {
            var paramsElement = new XElement("parameters");
            foreach (var (key, value) in message.Parameters)
                paramsElement.Add(new XElement(key, value));
            root.Add(paramsElement);
        }

        // Data
        if (_options.IncludeData && message.Data != null)
        {
            var metadata = new XElement("data");

            var paramDict = message.Data
            .GetType()
            .GetProperties()
            .ToDictionary(p => p.Name, p => p.GetValue(message.Data));
            foreach (var (key, value) in paramDict)
                    metadata.Add(new XElement(key, value));

            if (metadata.HasElements)
                root.Add(metadata);
        }

        // Metadata
        if (_options.IncludeMetadata)
        {
            var metadata = new XElement("metadata");

            if (_options.IncludeHttpStatusCode)
                metadata.Add(new XElement("httpStatusCode", message.HttpStatusCode));

            if (message.Metadata?.Count > 0)
            {
                foreach (var (key, value) in message.Metadata)
                    metadata.Add(new XElement(key, value));
            }

            if (metadata.HasElements)
                root.Add(metadata);
        }

        return root.ToString();
    }

    public object FormatAsObject(Message message)
    {
        return XDocument.Parse(Format(message));
    }
}
