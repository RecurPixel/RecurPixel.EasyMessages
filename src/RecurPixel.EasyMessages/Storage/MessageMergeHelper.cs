using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages.Storage;

/// <summary>
/// Helper for merging custom messages with defaults
/// </summary>
internal static class MessageMergeHelper
{
    /// <summary>
    /// Merges custom messages with defaults, completing partial templates
    /// </summary>
    public static Dictionary<string, MessageTemplate> MergeWithDefaults(
        Dictionary<string, MessageTemplate> defaults,
        Dictionary<string, MessageTemplate> custom
    )
    {
        var merged = new Dictionary<string, MessageTemplate>();

        foreach (var (code, customTemplate) in custom)
        {
            if (defaults.TryGetValue(code, out var defaultTemplate))
            {
                // Complete partial custom with default values
                merged[code] = MergeTemplates(defaultTemplate, customTemplate);
            }
            else
            {
                // New custom code (not in defaults)
                merged[code] = customTemplate;
            }
        }

        return merged;
    }

    internal static MessageTemplate MergeTemplates(
        MessageTemplate defaultTemplate,
        MessageTemplate customTemplate
    )
    {
        return new MessageTemplate
        {
            Type = customTemplate?.Type ?? defaultTemplate.Type,
            Title = customTemplate?.Title ?? defaultTemplate.Title,
            Description = customTemplate?.Description ?? defaultTemplate.Description,
            HttpStatusCode = customTemplate?.HttpStatusCode ?? defaultTemplate.HttpStatusCode,
            Hint = customTemplate?.Hint ?? defaultTemplate.Hint,
        };
    }
}
