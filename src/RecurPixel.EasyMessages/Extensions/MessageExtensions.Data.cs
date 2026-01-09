using RecurPixel.EasyMessages.Exceptions;
using RecurPixel.EasyMessages.Helpers;

namespace RecurPixel.EasyMessages;

/// <summary>
/// Message Extension Methods for adding Data
/// </summary>
public static partial class MessageExtensions
{
    /// <summary>
    /// Adds a hint string to the message.
    /// </summary>
    /// <param name="message">The message object to attach the hint to.</param>
    /// <param name="hint">The text of the hint to be added.</param>
    /// <returns>A new message instance with the updated Hint property.</returns>
    public static Message WithHint(this Message message, string hint)
    {
        return message with { Hint = hint };
    }

    /// <summary>
    /// Adds a data object to the message.
    /// </summary>
    /// <param name="message">The message object to attach the data to.</param>
    /// <param name="data">The object of the data to be added.</param>
    /// <returns>A new message instance with the updated data property.</returns>
    public static Message WithData(this Message message, object data)
    {
        return message with { Data = data };
    }

    /// <summary>
    /// Adds a correlationId string to the message.
    /// </summary>
    /// <param name="message">The message object to attach the correlationId to.</param>
    /// <param name="correlationId">The text of the correlationId to be added.</param>
    /// <returns>A new message instance with the updated correlationId property.</returns>
    public static Message WithCorrelationId(this Message message, string correlationId)
    {
        return message with { CorrelationId = correlationId };
    }

    /// <summary>
    /// Adds a Metadata object to the message.
    /// </summary>
    /// <param name="message">The message object to attach the Metadata to.</param>
    /// <param name="key">The metadata key.</param>
    /// <param name="value">The metadata value.</param>
    /// <returns>A new message instance with the updated metadata object.</returns>
    public static Message WithMetadata(this Message message, string key, object value)
    {
        // Optimize: Only create new dictionary if we actually have existing metadata
        var metadata =
            message.Metadata?.Count > 0
                ? new Dictionary<string, object>(message.Metadata)
                : new Dictionary<string, object>();
        metadata[key] = value;
        return message with { Metadata = metadata };
    }

    /// <summary>
    /// Adds a statusCode to the message.
    /// </summary>
    /// <param name="message">The message object to attach the statusCode to.</param>
    /// <param name="statusCode">The statusCode to be added.</param>
    /// <returns>A new message instance with the updated statusCode property.</returns>
    public static Message WithStatusCode(this Message message, int statusCode)
    {
        return message with { HttpStatusCode = statusCode };
    }

    /// <summary>
    /// Adds a parameters to the message and updates Message description with matching field name.
    /// </summary>
    /// <param name="message">The message object to attach the parameters to.</param>
    /// <param name="parameters">The parameters to be added.</param>
    /// <returns>A new message instance with the updated parameters property.</returns>
    public static Message WithParams(this Message message, object parameters)
    {
        if (!TypeExtensions.IsAnonymousType(parameters.GetType()))
        {
            throw new InvalidMessageParameterFileException("Invalid Parameter Type.");
        }

        var properties = parameters.GetType().GetProperties();
        var paramDict = new Dictionary<string, object?>(properties.Length);

        // Pre-allocate for better performance
        foreach (var prop in properties)
        {
            paramDict[prop.Name] = prop.GetValue(parameters);
        }

        // Only perform string replacements if there are parameters
        if (paramDict.Count == 0)
        {
            return message with { Parameters = paramDict };
        }

        var title = message.Title;
        var description = message.Description;

        foreach (var (key, value) in paramDict)
        {
            var placeholder = $"{{{key}}}";
            var replacement = value?.ToString() ?? string.Empty;

            // Optimize: Only replace if placeholder exists
            if (title.Contains(placeholder, StringComparison.OrdinalIgnoreCase))
            {
                title = title.Replace(placeholder, replacement, StringComparison.OrdinalIgnoreCase);
            }

            if (description.Contains(placeholder, StringComparison.OrdinalIgnoreCase))
            {
                description = description.Replace(
                    placeholder,
                    replacement,
                    StringComparison.OrdinalIgnoreCase
                );
            }
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
