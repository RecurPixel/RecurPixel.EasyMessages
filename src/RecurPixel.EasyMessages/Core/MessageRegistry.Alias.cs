using RecurPixel.EasyMessages.Storage;

namespace RecurPixel.EasyMessages.Core;

public static partial class MessageRegistry
{
    // Store-based loading (new flexible approach)
    [Obsolete("Use Configure(IMessageStore) instead")]
    public static void UseStore(IMessageStore store)
    {
        Configure(store);
    }

    // Composite store approach
    [Obsolete("Use Configure(new CompositeMessageStore(IMessageStore[])) instead")]
    public static void UseStores(params IMessageStore[] stores)
    {
        Configure(new CompositeMessageStore(stores));
    }

    // Legacy methods for backward compatibility (optional - can remove if not needed)
    [Obsolete("Use Configure(IMessageStore) instead")]
    public static void LoadCustomMessages(string jsonPath)
    {
        Configure(new FileMessageStore(jsonPath));
    }

    [Obsolete("Use Configure(IMessageStore) instead")]
    public static void LoadCustomMessages(Dictionary<string, MessageTemplate> messages)
    {
        Configure(new DictionaryMessageStore(messages));
    }
}
