using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages.Storage;

/// <summary>
/// Combines multiple stores with priority
/// Custom → Database → Defaults
/// </summary>
public class CompositeMessageStore : IMessageStore
{
    private readonly List<IMessageStore> _stores;

    public CompositeMessageStore(params IMessageStore[] stores)
    {
        _stores = stores.ToList();
    }

    public async Task<Dictionary<string, MessageTemplate>> LoadAsync()
    {
        var allMessages = new Dictionary<string, MessageTemplate>();

        // Load in reverse priority (last store wins)
        foreach (var store in _stores)
        {
            try
            {
                var messages = await store.LoadAsync();

                // Merge, overwriting duplicates
                foreach (var (code, template) in messages)
                {
                    allMessages[code] = template;
                }
            }
            catch (Exception ex)
            {
                // Log and continue with next store
                Console.WriteLine($"Failed to load from store: {ex.Message}");
            }
        }

        return allMessages;
    }
}


// // Priority: Custom file → Database → Defaults
// var store = new CompositeMessageStore(
//     new EmbeddedMessageStore(),      // Lowest priority (fallback)
//     new DatabaseMessageStore("..."), // Medium priority
//     new FileMessageStore("custom.json") // Highest priority (overrides)
// );

// var messages = await store.LoadAsync();
// // If custom.json has AUTH_001, it wins
// // Otherwise, check database
// // Otherwise, use embedded default