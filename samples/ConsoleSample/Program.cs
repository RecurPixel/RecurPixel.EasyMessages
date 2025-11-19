using RecurPixel.EasyMessages;

public class Class1
{
    public static void Main()
    {
        object data = new { Name = "Alpha 1", Version = "1.1" };

        
            var username = new {USERname = "abc@gmail.com", password = "pass"};


        var testrequired = Msg.Auth.LoginSuccess();

        testrequired = testrequired.WithCorrelationId("abc-xyz-11px-123-oo-00").WithMetadata("Hero","Lost").WithStatusCode(419)
        .WithParams(username);

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
