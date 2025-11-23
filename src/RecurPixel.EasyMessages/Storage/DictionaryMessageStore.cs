using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages.Storage;

/// <summary>
/// Loads messages from in-memory dictionary
/// </summary>
public class DictionaryMessageStore : IMessageStore
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DictionaryMessageStore"/> class with the specified messages.
    /// </summary>
    private readonly Dictionary<string, MessageTemplate> _messages;

    /// <summary>
    /// Creates dictionary-based message store
    /// </summary>
    /// <param name="messages">Takes Message Dictionary</param>

    public DictionaryMessageStore(Dictionary<string, MessageTemplate> messages)
    {
        _messages = messages ?? throw new ArgumentNullException(nameof(messages));
    }

    /// <summary>
    /// Loads messages from in-memory dictionary asynchronously
    /// </summary>
    /// <returns>Task of Message Dictionary</returns>
    public Task<Dictionary<string, MessageTemplate>> LoadAsync()
    {
        return Task.FromResult(new Dictionary<string, MessageTemplate>(_messages));
    }
}
