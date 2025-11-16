using System.Text.Json;
using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages.Storage;

/// <summary>
/// Loads messages from external JSON files
/// </summary>
public class FileMessageStore : IMessageStore
{
    private readonly string _filePath;

    public FileMessageStore(string filePath)
    {
        _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
    }

    public async Task<Dictionary<string, MessageTemplate>> LoadAsync()
    {
        if (!File.Exists(_filePath))
            throw new FileNotFoundException($"Message file not found: {_filePath}");

        var json = await File.ReadAllTextAsync(_filePath);

        return JsonSerializer.Deserialize<Dictionary<string, MessageTemplate>>(json)
            ?? new Dictionary<string, MessageTemplate>();
    }

    public Task<bool> IsAvailableAsync()
    {
        return Task.FromResult(File.Exists(_filePath));
    }
}

// // Load custom messages from file
// var store = new FileMessageStore("messages/custom.json");
// var messages = await store.LoadAsync();