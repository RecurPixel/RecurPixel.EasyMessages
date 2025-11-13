using System.Text.Json;

namespace RecurPixel.EasyMessages.Core;

public static class MessageRegistry
{
    private static readonly Lazy<Dictionary<string, MessageTemplate>> _defaults =
        new(() => LoadEmbeddedMessages());
    
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
            $"Message code '{code}' not found in registry. " +
            $"Available codes: {string.Join(", ", GetAllCodes().Take(10))}...");
    }
    
    public static void LoadCustomMessages(string jsonPath)
    {
        lock (_lock)
        {
            var json = File.ReadAllText(jsonPath);
            _custom = JsonSerializer.Deserialize<Dictionary<string, MessageTemplate>>(json);
        }
    }
    
    internal static void LoadCustomMessages(Dictionary<string, MessageTemplate> messages)
    {
        lock (_lock)
        {
            _custom = messages;
        }
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
        var resourceName = "EasyMessages.Core.Messages.defaults.json";
        
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
            throw new InvalidOperationException($"Embedded resource '{resourceName}' not found");
        
        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();
        
        return JsonSerializer.Deserialize<Dictionary<string, MessageTemplate>>(json)
            ?? throw new InvalidOperationException("Failed to deserialize default messages");
    }
}

public class MessageNotFoundException : Exception
{
    public MessageNotFoundException(string message) : base(message) { }
}