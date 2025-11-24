using RecurPixel.EasyMessages.Core;
using RecurPixel.EasyMessages.Storage;

namespace RecurPixel.EasyMessages;

/// <summary>
/// Alias methods for MessageRegistry for backward compatibility
/// </summary>
public static partial class MessageRegistry
{
    /// <summary>
    /// Configures the message store to be used by the MessageRegistry.
    /// </summary>
    /// <param name="store">A Messages Store of Type IMessageStore</param>
    /// <remarks>
    /// This method is a **convenience wrapper** that creates a
    /// <c>DictionaryMessageStore</c> and calls the primary
    /// <see cref="Configure(IMessageStore)"/> method.
    /// </remarks>
    public static void UseStore(IMessageStore store)
    {
        Configure(store);
    }

    /// <summary>
    /// Configures multiple message stores to be used by the MessageRegistry.
    /// </summary>
    /// <param name="stores">Takes list of IMessageStore</param>
    /// <remarks>
    /// This method is a **convenience wrapper** that creates a
    /// <c>DictionaryMessageStore</c> and calls the primary
    /// <see cref="Configure(IMessageStore)"/> method.
    /// </remarks>
    public static void UseStores(params IMessageStore[] stores)
    {
        Configure(new CompositeMessageStore(stores));
    }

    /// <summary>
    /// Loads custom messages from a JSON file.
    /// </summary>
    /// <param name="jsonPath">Takes path to json File</param>
    /// <remarks>
    /// This method is a **convenience wrapper** that creates a
    /// <c>DictionaryMessageStore</c> and calls the primary
    /// <see cref="Configure(IMessageStore)"/> method.
    /// </remarks>
    public static void LoadCustomMessages(string jsonPath)
    {
        Configure(new FileMessageStore(jsonPath));
    }

    /// <summary>
    /// Loads custom messages from an in-memory dictionary.
    /// </summary>
    /// <param name="messages">The dictionary of custom messages to load.</param>
    /// <remarks>
    /// This method is a **convenience wrapper** that creates a
    /// <c>DictionaryMessageStore</c> and calls the primary
    /// <see cref="Configure(IMessageStore)"/> method.
    /// </remarks>
    public static void LoadCustomMessages(Dictionary<string, MessageTemplate> messages)
    {
        Configure(new DictionaryMessageStore(messages));
    }
}
