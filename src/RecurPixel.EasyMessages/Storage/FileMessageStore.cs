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

    /// <summary>
    /// Creates file-based message store
    /// </summary>
    /// <param name="filePath">Takes File Path</param>
    public FileMessageStore(string filePath)
    {
        _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
    }

    /// <summary>
    /// Loads messages from file asynchronously
    /// </summary>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="InvalidMessageFileException"></exception>
    /// <returns>Returns a Task</returns>

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
