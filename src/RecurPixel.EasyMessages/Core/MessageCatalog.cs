using System.Text.Json;
using System.Text.Json.Serialization;

namespace RecurPixel.EasyMessages.Core;

public class MessageCatalog
{
    // These properties map to the root of the JSON

    [JsonPropertyName("$schema")]
    public string Schema { get; set; } // Maps to "$schema"

    [JsonPropertyName("version")]
    public string Version { get; set; }

    // This property handles the dynamic "messages" section
    
    [JsonPropertyName("messages")]
    public Dictionary<string, System.Text.Json.JsonElement> RawMessages { get; set; } = new();

    internal Dictionary<string, MessageTemplate> Messages
    {
        get
        {
            // The method that isolates the cleaning logic
            return ConvertRawMessages(this.RawMessages);
        }
    }
    
    private Dictionary<string, MessageTemplate> ConvertRawMessages(
        Dictionary<string, System.Text.Json.JsonElement> rawData)
    {
        var cleanMessages = new Dictionary<string, MessageTemplate>();
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,

            Converters = { new JsonStringEnumConverter() } 
        };
        
        foreach (var kvp in rawData)
        {

            if (kvp.Value.ValueKind == System.Text.Json.JsonValueKind.Object)
            {
                // Deserialize the valid object into your MessageTemplate type

                try
                {
                    
                    var message = kvp.Value.Deserialize<MessageTemplate>(options);

                    if (message != null)
                    {
                        cleanMessages.TryAdd(kvp.Key, message);
                    }                    
                }catch(Exception ex)
                {
                    Console.WriteLine($"Error Occured: {ex.Message}");
                }

            }
        }

        return cleanMessages;
    }
}