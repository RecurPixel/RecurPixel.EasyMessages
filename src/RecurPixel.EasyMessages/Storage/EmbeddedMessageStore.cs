using System.Text.Json;
using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages.Storage;

/// <summary>
/// Loads default messages embedded in the assembly
/// </summary>
public class EmbeddedMessageStore : IMessageStore
{
    private const string ResourceName = "RecurPixel.EasyMessages.Messages.defaults.json";

    public Task<Dictionary<string, MessageTemplate>> LoadAsync()
    {
        var assembly = typeof(EmbeddedMessageStore).Assembly;

        using var stream = assembly.GetManifestResourceStream(ResourceName);
        if (stream == null)
            throw new InvalidOperationException($"Embedded resource '{ResourceName}' not found");

        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();

        var catalog = MessageCatalog.FromJson(json);

        var messages =
            catalog?.Messages
            ?? throw new InvalidOperationException("Failed to deserialize default messages");

        return Task.FromResult(messages);
    }
}
