namespace RecurPixel.EasyMessages.Core;

public sealed class MessageTemplate
{
    public MessageType Type { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public int? HttpStatusCode { get; set; }

    public string? Hint { get; set; }

    public Message ToMessage(string code)
    {
        return new Message
        {
            Code = code,
            Type = Type,
            Title = Title ?? string.Empty,
            Description = Description ?? string.Empty,
            HttpStatusCode = HttpStatusCode,
            Hint = Hint,
        };
    }
}
