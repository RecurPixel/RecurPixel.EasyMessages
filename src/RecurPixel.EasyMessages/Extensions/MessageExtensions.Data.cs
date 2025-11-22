using RecurPixel.EasyMessages.Exceptions;
using RecurPixel.EasyMessages.Helpers;

namespace RecurPixel.EasyMessages;

public static partial class MessageExtensions
{
    public static Message WithHint(this Message message, string hint)
    {
        return message with { Hint = hint };
    }

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

            title = title.Replace(placeholder, replacement, StringComparison.OrdinalIgnoreCase);
            description = description.Replace(
                placeholder,
                replacement,
                StringComparison.OrdinalIgnoreCase
            );
        }

        return message with
        {
            Title = title,
            Description = description,
            Parameters = paramDict,
        };
    }

    /// <summary>
    /// Apply parameters only if provided (non-null values)
    /// </summary>
    public static Message WithParamsIfProvided(this Message message, object? parameters)
    {
        if (parameters == null)
            return message;

        // Filter out null properties
        var properties = parameters
            .GetType()
            .GetProperties()
            .Where(p => p.GetValue(parameters) != null)
            .ToDictionary(p => p.Name, p => p.GetValue(parameters)!);

        if (!properties.Any())
            return message;

        // Create anonymous object with non-null values
        var nonNullParams = properties.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        return message.WithParams(nonNullParams);
    }
}
