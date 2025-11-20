using RecurPixel.EasyMessages.Exceptions;
using RecurPixel.EasyMessages.Storage;

namespace RecurPixel.EasyMessages.Core;

public static partial class MessageRegistry
{
    private static readonly Lazy<Dictionary<string, MessageTemplate>> _defaults = new(() =>
        LoadDefaultsSync()
    );

    private static Dictionary<string, MessageTemplate>? _custom;
    private static readonly object _lock = new();

    /// <summary>
    /// Configure custom message store(s)
    /// Defaults are always used as fallback
    /// </summary>
    public static void Configure(IMessageStore store)
    {
        if (store == null)
            throw new ArgumentNullException(nameof(store));

        lock (_lock)
        {
            try
            {
                // Load from store (can be single or composite)
                var customMessages = store.LoadAsync().GetAwaiter().GetResult();

                // Merge with defaults to complete partial templates
                _custom = MessageMergeHelper.MergeWithDefaults(_defaults.Value, customMessages);

                Console.WriteLine($"Successfully configured {_custom.Count} custom message(s)");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to configure message store: {ex.Message}",
                    ex
                );
            }
        }
    }

    public static Message Get(string code)
    {
        // 1. Check custom first (already merged with defaults)
        if (_custom?.TryGetValue(code, out var customTemplate) == true)
            return customTemplate.ToMessage(code);

        // 2. Fallback to defaults
        if (_defaults.Value.TryGetValue(code, out var defaultTemplate))
            return defaultTemplate.ToMessage(code);

        // 3. Not found
        throw new MessageNotFoundException(
            $"Message code '{code}' not found. Available: {string.Join(", ", GetAllCodes().Take(10))}..."
        );
    }

    public static IEnumerable<string> GetAllCodes()
    {
        var codes = new HashSet<string>(_defaults.Value.Keys);
        if (_custom != null)
            codes.UnionWith(_custom.Keys);
        return codes.OrderBy(c => c);
    }

    private static Dictionary<string, MessageTemplate> LoadDefaultsSync()
    {
        var store = new EmbeddedMessageStore();
        return store.LoadAsync().GetAwaiter().GetResult();
    }
}
