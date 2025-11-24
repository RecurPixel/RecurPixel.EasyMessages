using System.Collections.Immutable;
using System.Threading;
using RecurPixel.EasyMessages.Core;
using RecurPixel.EasyMessages.Exceptions;
using RecurPixel.EasyMessages.Storage;

namespace RecurPixel.EasyMessages;

/// <summary>
/// (Message Stores)Registry for managing message templates and retrieval
/// </summary>
public static partial class MessageRegistry
{
    private static readonly Lazy<Dictionary<string, MessageTemplate>> _defaults = new(() =>
        LoadDefaultsSync()
    );

    // Use an immutable dictionary for thread-safe snapshot semantics
    private static ImmutableDictionary<string, MessageTemplate>? _custom;

    /// <summary>
    /// Configure custom message store(s)
    /// Defaults are always used as fallback
    /// </summary>
    public static void Configure(IMessageStore store)
    {
        if (store == null)
            throw new ArgumentNullException(nameof(store));

        try
        {
            // Load from store (can be single or composite)
            var customMessages = store.LoadAsync().GetAwaiter().GetResult();

            // Merge with defaults to complete partial templates
            var merged = MessageMergeHelper.MergeWithDefaults(_defaults.Value, customMessages);

            // Convert to immutable and swap atomically
            var immutable = merged.ToImmutableDictionary();
            Interlocked.Exchange(ref _custom, immutable);

            Console.WriteLine($"Successfully configured {_custom?.Count ?? 0} custom message(s)");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to configure message store: {ex.Message}",
                ex
            );
        }
    }

    /// <summary>
    /// Gets the message by code, checking custom messages first, then defaults.
    /// </summary>
    /// <param name="code">Message Code</param>
    /// <returns>Message Object</returns>
    public static Message Get(string code)
    {
        // 1. Check custom first (already merged with defaults)
        var snapshot = _custom;
        if (snapshot != null && snapshot.TryGetValue(code, out var customTemplate))
            return customTemplate.ToMessage(code);

        // 2. Fallback to defaults
        if (_defaults.Value.TryGetValue(code, out var defaultTemplate))
            return defaultTemplate.ToMessage(code);

        // 3. Not found
        throw new MessageNotFoundException(
            $"Message code '{code}' not found. Available: {string.Join(", ", GetAllCodes().Take(10))}..."
        );
    }

    /// <summary>
    ///  Gets all available message codes from both custom and default messages.
    /// </summary>
    public static IEnumerable<string> GetAllCodes()
    {
        var codes = new HashSet<string>(_defaults.Value.Keys);
        var snapshot = _custom;
        if (snapshot != null)
            codes.UnionWith(snapshot.Keys);
        return codes.OrderBy(c => c);
    }

    /// <summary>
    /// Loads the default messages from the embedded store synchronously.
    /// </summary>
    private static Dictionary<string, MessageTemplate> LoadDefaultsSync()
    {
        var store = new EmbeddedMessageStore();
        return store.LoadAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Reset Custom Messages (for testing purposes)
    /// </summary>
    public static void Reset()
    {
        Interlocked.Exchange(ref _custom, null);
    }
}
