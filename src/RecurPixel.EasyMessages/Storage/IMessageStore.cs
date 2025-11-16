using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages.Storage;

/// <summary>
/// Defines how messages are loaded from any source
/// </summary>
public interface IMessageStore
{
    /// <summary>
    /// Load all messages from the store
    /// </summary>
    internal Task<Dictionary<string, MessageTemplate>> LoadAsync();

    /// <summary>
    /// Check if store is available (optional operation)
    /// </summary>
    Task<bool> IsAvailableAsync() => Task.FromResult(true);
}
