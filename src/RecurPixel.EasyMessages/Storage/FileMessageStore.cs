using System.Text.Json;
using RecurPixel.EasyMessages.Core;
using RecurPixel.EasyMessages.Exceptions;

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

        try
        {
            var json = await File.ReadAllTextAsync(_filePath);
            var catalog = MessageCatalog.FromJson(json);
            return catalog?.Messages ?? new Dictionary<string, MessageTemplate>();
        }
        catch (JsonException ex)
        {
            throw new InvalidMessageFileException(
                "Message: \"Custom messages file contains invalid JSON\"\nCause: Malformed custom JSON file",
                ex
            );
        }
    }
}
