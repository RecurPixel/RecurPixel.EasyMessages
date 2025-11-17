using System.Text.Json;
using RecurPixel.EasyMessages.Exceptions;

namespace RecurPixel.EasyMessages.Core;

public static class MessageRegistry
{
    private static readonly Lazy<Dictionary<string, MessageTemplate>> _defaults = new(() =>
        LoadEmbeddedMessages()
    );

    private static Dictionary<string, MessageTemplate>? _custom;
    private static readonly object _lock = new();

    public static Message Get(string code)
    {
        // Check custom first
        if (_custom?.TryGetValue(code, out var customTemplate) == true)
            return customTemplate.ToMessage(code);

        // Fall back to defaults
        if (_defaults.Value.TryGetValue(code, out var defaultTemplate))
            return defaultTemplate.ToMessage(code);

        // Not found
        throw new MessageNotFoundException(
            $"Message code '{code}' not found in registry. "
                + $"Available codes: {string.Join(", ", GetAllCodes().Take(10))}..."
        );
    }

    public static void LoadCustomMessages(string jsonPath)
    {
        if (!File.Exists(jsonPath))
        {
            throw new FileNotFoundException($"Custom message file not found at path: {jsonPath}");
        }

        lock (_lock)
        {
            try
            {
                var json = File.ReadAllText(jsonPath);
                var catalog = MessageCatalog.FromJson(json);
                LoadCustomMessagesInternal(catalog.Messages);

                Console.WriteLine(
                    $"Successfully loaded {_custom.Count} custom message(s) from '{jsonPath}'"
                );
            }
            catch (InvalidMessageFileException ex)
            {
                throw new InvalidMessageFileException(
                    $"Failed to load custom messages from '{jsonPath}': {ex.Message}",
                    ex
                );
            }
            catch (IOException ex)
            {
                throw new InvalidMessageFileException(
                    $"Failed to read custom message file '{jsonPath}': {ex.Message}",
                    ex
                );
            }
        }
    }

    public static void LoadCustomMessages(Dictionary<string, MessageTemplate> messages)
    {
        if (messages == null || messages.Count == 0)
        {
            throw new ArgumentException(
                "Custom messages dictionary cannot be null or empty.",
                nameof(messages)
            );
        }

        lock (_lock)
        {
            LoadCustomMessagesInternal(messages);
            Console.WriteLine(
                $"Successfully loaded {_custom.Count} custom message(s) from dictionary"
            );
        }
    }

    private static void LoadCustomMessagesInternal(Dictionary<string, MessageTemplate> messages)
    {
        _custom = new Dictionary<string, MessageTemplate>();
        var warnings = new List<string>();

        foreach (var (code, customTemplate) in messages)
        {
            if (_defaults.Value.TryGetValue(code, out var defaultTemplate))
            {
                _custom[code] = MergeTemplates(defaultTemplate, customTemplate);
            }
            else
            {
                _custom[code] = customTemplate;
                warnings.Add($"'{code}': No default template found, using custom as-is");
            }
        }

        // Optional: Log warnings about codes not in defaults
        if (warnings.Any())
        {
            Console.WriteLine(
                $"Note: {warnings.Count} custom message code(s) not found in defaults:\n"
                    + string.Join("\n", warnings.Select(w => $"  - {w}"))
            );
        }
    }

    private static MessageTemplate MergeTemplates(
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

    public static IEnumerable<string> GetAllCodes()
    {
        var codes = new HashSet<string>(_defaults.Value.Keys);
        if (_custom != null)
            codes.UnionWith(_custom.Keys);
        return codes.OrderBy(c => c);
    }

    private static Dictionary<string, MessageTemplate> LoadEmbeddedMessages()
    {
        var assembly = typeof(MessageRegistry).Assembly;
        var resourceName = "RecurPixel.EasyMessages.Messages.defaults.json";

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
            throw new InvalidOperationException($"Embedded resource '{resourceName}' not found");

        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();

        return JsonSerializer.Deserialize<MessageCatalog>(json)?.Messages
            ?? throw new InvalidOperationException("Failed to deserialize default messages");
    }
}
