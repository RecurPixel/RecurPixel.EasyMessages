using RecurPixel.EasyMessages.Core;


namespace RecurPixel.EasyMessages.Core.Extensions;

public static class MessageExtensions
{
    public static Message WithData(this Message message, object data)
    {
        return message with { Data = data };
    }
    
    public static Message WithCorrelationId(this Message message, string correlationId)
    {
        return message with { CorrelationId = correlationId };
    }
    
    public static Message WithMetadata(this Message message, string key, object value)
    {
        var metadata = new Dictionary<string, object>(message.Metadata ?? new());
        metadata[key] = value;
        return message with { Metadata = metadata };
    }
    
    public static Message WithStatusCode(this Message message, int statusCode)
    {
        return message with { HttpStatusCode = statusCode };
    }
    
/*
    **Parameter Substitution Specification:**
// Rules:
// 1. Placeholders: {fieldName} - case-insensitive matching
// 2. Missing params: Leave placeholder unchanged (e.g., "{field} is required")
// 3. Type conversion: Always .ToString() on parameter values
// 4. No formatting in v1.0: {amount:C} NOT supported
// 5. Null values: Convert to empty string

// Examples:
var msg1 = MessageRegistry.Get("VAL_002")
    .WithParams(new { Field = "Email" });      // "Email is required"
    
var msg2 = MessageRegistry.Get("VAL_002")
    .WithParams(new { field = "Email" });      // Same result (case-insensitive)
    
var msg3 = MessageRegistry.Get("VAL_002")
    .WithParams(new { WrongParam = "X" });     // "{field} is required" (unchanged)

*/
    
    public static Message WithParams(this Message message, object parameters)
    {
        var properties = parameters.GetType().GetProperties();
        var title = message.Title;
        var description = message.Description;
        
        foreach (var prop in properties)
        {
            var value = prop.GetValue(parameters)?.ToString() ?? "";
            var placeholder = $"{{{prop.Name}}}";
            
            title = title.Replace(placeholder, value);
            description = description.Replace(placeholder, value);
        }
        
        return message with 
        { 
            Title = title, 
            Description = description 
        };
    }
}