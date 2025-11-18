using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages.Storage;

/// <summary>
/// Loads messages from in-memory dictionary
/// </summary>
public class DictionaryMessageStore : IMessageStore
{
    private readonly Dictionary<string, MessageTemplate> _messages;

    public DictionaryMessageStore(Dictionary<string, MessageTemplate> messages)
    {
        _messages = messages ?? throw new ArgumentNullException(nameof(messages));
    }

    public Task<Dictionary<string, MessageTemplate>> LoadAsync()
    {
        return Task.FromResult(new Dictionary<string, MessageTemplate>(_messages));
    }
}
