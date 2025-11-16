using System.Text.Json;
using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages.Storage;

/// <summary>
/// Loads default messages embedded in the assembly
/// </summary>
public class EmbeddedMessageStore : IMessageStore
{
    private const string ResourceName = "RecurPixel.EasyMessages.Messages.defaults.json";

    public async Task<Dictionary<string, MessageTemplate>> LoadAsync()
    {
        var assembly = typeof(EmbeddedMessageStore).Assembly;

        await using var stream = assembly.GetManifestResourceStream(ResourceName);
        if (stream == null)
            throw new InvalidOperationException($"Embedded resource '{ResourceName}' not found");

        using var reader = new StreamReader(stream);
        var json = await reader.ReadToEndAsync();

        return JsonSerializer.Deserialize<Dictionary<string, MessageTemplate>>(json)
            ?? throw new InvalidOperationException("Failed to deserialize default messages");
    }
}


// // Zero configuration - Just works!
// var store = new EmbeddedMessageStore();
// var messages = await store.LoadAsync(); // Loads from embedded defaults.json