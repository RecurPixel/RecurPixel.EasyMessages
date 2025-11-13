namespace RecurPixel.EasyMessages.Core;

internal sealed class MessageTemplate
{
    public MessageType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? HttpStatusCode { get; set; }
    
    public Message ToMessage(string code)
    {
        return new Message
        {
            Code = code,
            Type = Type,
            Title = Title,
            Description = Description,
            HttpStatusCode = HttpStatusCode ?? GetDefaultStatusCode(Type)
        };
    }
    
    private static int GetDefaultStatusCode(MessageType type)
    {
        return type switch
        {
            MessageType.Success => 200,
            MessageType.Info => 200,
            MessageType.Warning => 200,
            MessageType.Error => 400,
            MessageType.Critical => 500,
            _ => 200
        };
    }
}