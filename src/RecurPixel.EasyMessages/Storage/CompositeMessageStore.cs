using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages.Storage;

/// <summary>
/// Combines multiple stores with priority (last store = highest priority)
/// </summary>
public class CompositeMessageStore : IMessageStore
{
    private readonly List<IMessageStore> _stores;

    /// <summary>
    /// Creates composite store with priority order (first = lowest, last = highest)
    /// </summary>
    public CompositeMessageStore(params IMessageStore[] stores)
    {
        if (stores == null || stores.Length == 0)
            throw new ArgumentException("At least one store must be provided", nameof(stores));

        _stores = stores.ToList();
    }

    /// <summary>
    /// Loads messages from all stores, merging them with priority
    /// </summary>
    /// <returns></returns>
    public async Task<Dictionary<string, MessageTemplate>> LoadAsync()
    {
        var allMessages = new Dictionary<string, MessageTemplate>();

        foreach (var store in _stores)
        {
            try
            {
                var messages = await store.LoadAsync();

                foreach (var (code, template) in messages)
                {
                    if (allMessages.TryGetValue(code, out var existing))
                    {
                        // Progressive merge - later stores override only non-null fields
                        allMessages[code] = MessageMergeHelper.MergeTemplates(existing, template);
                    }
                    else
                    {
                        allMessages[code] = template;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to load from store: {ex.Message}");
            }
        }

        return allMessages;
    }
}
