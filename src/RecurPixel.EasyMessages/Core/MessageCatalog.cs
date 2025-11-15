using System.Text.Json;
using System.Text.Json.Serialization;
using RecurPixel.EasyMessages.Exceptions;

namespace RecurPixel.EasyMessages.Core;

public class MessageCatalog
{
    [JsonPropertyName("$schema")]
    public string Schema { get; set; }

    [JsonPropertyName("version")]
    public string Version { get; set; }

    [JsonPropertyName("messages")]
    public Dictionary<string, JsonElement> RawMessages { get; set; } = new();

    internal Dictionary<string, MessageTemplate> Messages
    {
        get => ConvertRawMessages(this.RawMessages);
    }

    public static MessageCatalog FromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new InvalidMessageFileException("Message file is empty or null.");
        }

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() },
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip
        };

        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // Check if it's valid JSON object
            if (root.ValueKind != JsonValueKind.Object)
            {
                throw new InvalidMessageFileException(
                    "Message file must be a JSON object at the root level.");
            }

            // Full format with "messages" property
            if (root.TryGetProperty("messages", out var messagesProperty))
            {
                if (messagesProperty.ValueKind != JsonValueKind.Object)
                {
                    throw new InvalidMessageFileException(
                        "The 'messages' property must be a JSON object containing message definitions.");
                }

                var catalog = JsonSerializer.Deserialize<MessageCatalog>(json, options);
                
                if (catalog == null || catalog.RawMessages == null || catalog.RawMessages.Count == 0)
                {
                    throw new InvalidMessageFileException(
                        "No valid messages found in the 'messages' section.");
                }

                return catalog;
            }

            // Simple format - check for common typos
            if (root.TryGetProperty("message", out var _) || 
                root.TryGetProperty("mssages", out var _) ||
                root.TryGetProperty("mesages", out var _))
            {
                throw new InvalidMessageFileException(
                    "Found typo in property name. Did you mean 'messages'? " +
                    "For full format use: { \"messages\": { ... } }. " +
                    "For simple format, put message codes directly at root level.");
            }

            // Simple format - root is the messages dictionary
            var rawMessages = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json, options);
            
            if (rawMessages == null || rawMessages.Count == 0)
            {
                throw new InvalidMessageFileException(
                    "No valid message definitions found. " +
                    "Ensure your JSON contains message codes as keys with message objects as values.");
            }

            return new MessageCatalog
            {
                RawMessages = rawMessages
            };
        }
        catch (JsonException ex)
        {
            throw new InvalidMessageFileException(
                $"Malformed JSON in message file: {ex.Message}. " +
                $"Please check your JSON syntax at line {ex.LineNumber}, position {ex.BytePositionInLine}.",
                ex);
        }
    }

    private Dictionary<string, MessageTemplate> ConvertRawMessages(
        Dictionary<string, JsonElement> rawData)
    {
        var cleanMessages = new Dictionary<string, MessageTemplate>();
        var errors = new List<string>();
        
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        };

        foreach (var kvp in rawData)
        {
            // Skip comment entries
            if (kvp.Key.StartsWith("_"))
                continue;

            if (kvp.Value.ValueKind != JsonValueKind.Object)
            {
                errors.Add($"'{kvp.Key}': Expected an object but got {kvp.Value.ValueKind}");
                continue;
            }

            try
            {
                var message = kvp.Value.Deserialize<MessageTemplate>(options);
                
                if (message != null)
                {
                    cleanMessages.TryAdd(kvp.Key, message);
                }
                else
                {
                    errors.Add($"'{kvp.Key}': Failed to deserialize message template");
                }
            }
            catch (JsonException ex)
            {
                errors.Add($"'{kvp.Key}': {ex.Message}");
            }
        }

        // If we have errors but also some valid messages, log warnings
        if (errors.Any())
        {
            var errorMessage = $"Warning: {errors.Count} message(s) could not be parsed:\n" +
                             string.Join("\n", errors.Select(e => $"  - {e}"));
            Console.WriteLine(errorMessage);
        }

        // If NO valid messages were parsed, throw exception
        if (cleanMessages.Count == 0)
        {
            throw new InvalidMessageFileException(
                "No valid messages could be parsed from the file. Errors:\n" +
                string.Join("\n", errors.Select(e => $"  - {e}")));
        }

        return cleanMessages;
    }
}