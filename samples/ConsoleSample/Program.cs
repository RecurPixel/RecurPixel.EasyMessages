using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.Core.Extensions;

public class Class1
{
    public static void Main()
    {
        object data = new { Name = "Alpha 1", Version = "1.1" };

        var test = Msg.Auth.LoginFailed();

        test = test.WithData(data).WithCorrelationId("abc-xyz-11px-123-oo-00").WithMetadata("Hero","Lost").WithStatusCode(419);

        test.ToConsole();
        Console.WriteLine("\n");

        var json = test.ToJson();

        Console.WriteLine(json);

        var jsonO = test.ToJsonObject();

        Console.WriteLine(jsonO);

        var plainText = test.ToPlainText();

        Console.WriteLine(plainText);

        var xml = test.ToXml();

        Console.WriteLine(xml);

        var xmld = test.ToXmlDocument();

        Console.WriteLine(xmld);
    }
}
