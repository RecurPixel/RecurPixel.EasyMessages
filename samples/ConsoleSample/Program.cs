using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.Interceptors;

public class TimestampInterceptor : IMessageInterceptor
{
    public Message OnBeforeFormat(Message message)
    {
        Console.WriteLine($"[{DateTime.UtcNow:HH:mm:ss}] Formatting message {message.Code}");
        return message;
    }
    
    public Message OnAfterFormat(Message message) => message with { HttpStatusCode = 999 };
}

public class Class1
{
    public static void Main()
    {

        // Register
        InterceptorRegistry.Register(new TimestampInterceptor());

        object data = new { Name = "Alpha 1", Version = "1.1" };

        var username = new { USERname = "abc@gmail.com"};

        var testrequired = Msg.Validation.InvalidFormat("TestR");

        testrequired = testrequired
            .WithCorrelationId("abc-xyz-11px-123-oo-00")
            .WithMetadata("Hero", "Lost")
            .WithStatusCode(419)
            .WithHint("Please provide a valid format.");

        testrequired.ToConsole();
        Console.WriteLine("\n");

        var json = testrequired.ToJson();

        Console.WriteLine(json);

        var jsonO = testrequired.ToJsonObject();

        Console.WriteLine(jsonO);

        var plainText = testrequired.ToPlainText();

        Console.WriteLine(plainText);

        var xml = testrequired.ToXml();

        Console.WriteLine(xml);

        var xmld = testrequired.ToXmlDocument();

        Console.WriteLine(xmld);
    }
}
