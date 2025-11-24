using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.Configuration;
using RecurPixel.EasyMessages.Formatters;
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

        // Configure default formatter options
        FormatterConfiguration.SetDefaultOptions(
            new FormatterOptions { IncludeTimestamp = true, IncludeHttpStatusCode = true }
        );

        MessageRegistry.LoadCustomMessages("custom.json");

        Msg.Auth.LoginFailed().ToConsole();

        Msg.Custom("CUSTOM_ROOT_001").ToConsole();

        var msg = Msg.Crud.Created("Order").ToJson(FormatterConfiguration.Verbose);
        Console.WriteLine(msg);

        var msg2 = Msg.Validation.InvalidEmail().ToXml(FormatterConfiguration.Minimal);
        Console.WriteLine(msg2);
    }
}
