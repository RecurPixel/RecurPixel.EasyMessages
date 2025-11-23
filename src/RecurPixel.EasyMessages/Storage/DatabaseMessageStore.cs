using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages.Storage;

/// <summary>
/// Loads messages from database. Abstract Class
/// </summary>
public abstract class DatabaseMessageStore : IMessageStore
{
    /// <summary>
    /// Loads messages from database asynchronously
    /// </summary>
    /// <returns></returns>
    public abstract Task<Dictionary<string, MessageTemplate>> LoadAsync();
}


// // Enterprise: Centralized message management
// var store = new DatabaseMessageStore("Server=...;Database=Messages");
// var messages = await store.LoadAsync();
