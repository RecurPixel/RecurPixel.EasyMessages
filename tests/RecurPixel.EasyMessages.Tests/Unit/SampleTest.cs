using System.Reflection;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.Core;
using RecurPixel.EasyMessages.Exceptions;
using RecurPixel.EasyMessages.Storage;

namespace RecurPixel.EasyMessages.Tests.Unit;

[Collection("MessageRegistry")]
public class SampleTest : UnitTestBase
{
    public SampleTest() { }

    [Fact]
    public void Message_ShouldHaveTimestamp()
    {
        Dispose();
        var messageTemplate = new MessageTemplate
        {
            Type = MessageType.Success,
            Title = "Test",
            Description = "Test Message Templatge",
            HttpStatusCode = 200,
            Hint = "Guess",
        };
        Assert.True(
            (DateTime.UtcNow - messageTemplate.ToMessage("TEST_001").Timestamp).TotalSeconds < 1
        );
    }

    [Fact]
    public void Registry_ShouldLoadDefaultMessages()
    {
        Dispose();
        var message = MessageRegistry.Get("AUTH_001");
        Assert.Equal("AUTH_001", message.Code);
        Assert.Equal("Authentication Failed", message.Title);
    }

    [Fact]
    public void MessageCodes_Constants_ShouldMatchDefaultsJson()
    {
        Dispose();
        // Ensure MessageCodes.cs stays in sync with defaults.json
        var constantCodes = typeof(MessageCodes)
            .GetNestedTypes()
            .SelectMany(t => t.GetFields(BindingFlags.Public | BindingFlags.Static))
            .Where(f => f.FieldType == typeof(string))
            .Select(f => f.GetValue(null) as string)
            .Where(s => s != null)
            .ToHashSet();

        var jsonCodes = MessageRegistry.GetAllCodes().ToHashSet();

        // All constants must exist in JSON
        var missing = constantCodes.Except(jsonCodes).ToList();
        Assert.Empty(missing); // Fail if MessageCodes has codes not in defaults.json
    }

    [Fact]
    public void Registry_CustomShouldOverrideDefault()
    {
        Dispose();
        var custom = new Dictionary<string, MessageTemplate>
        {
            ["AUTH_001"] = new() { Title = "Custom Title", Description = "Custom" },
        };
        MessageRegistry.LoadCustomMessages(custom);

        var message = MessageRegistry.Get("AUTH_001");
        Assert.Equal("Custom Title", message.Title);
    }

    [Fact]
    public void Registry_ShouldThrowForInvalidCode()
    {
        Dispose();
        Assert.Throws<MessageNotFoundException>(() => MessageRegistry.Get("INVALID_999"));
    }

    [Fact]
    public void WithData_ShouldAddData()
    {
        Dispose();
        var message = MessageRegistry.Get("AUTH_003").WithData(new { UserId = 123 });

        Assert.NotNull(message.Data);
    }

    [Fact]
    public void WithParams_ShouldReplaceTemplateValues()
    {
        Dispose();
        var message = MessageRegistry.Get("CRUD_001").WithParams(new { resource = "User" });

        // Accept small variations in the template text (e.g. suffixes)
        Assert.Contains("User has been created", message.Description);
    }

    [Fact]
    public void Msg_Auth_LoginFailed_ShouldWork()
    {
        Dispose();
        var message = Msg.Auth.LoginFailed();
        Assert.Equal("AUTH_001", message.Code);
    }

    [Fact]
    public void Msg_Crud_Created_ShouldReplaceResource()
    {
        Dispose();
        var message = Msg.Crud.Created("User");
        Assert.Contains("User", message.Description);
    }

    [Fact]
    public void Msg_Custom_ShouldAccessAnyCode()
    {
        Dispose();
        var message = Msg.Custom("AUTH_001");
        Assert.Equal("AUTH_001", message.Code);
    }

    [Fact]
    public void ToJson_ShouldSerializeCorrectly()
    {
        Dispose();
        var message = Msg.Auth.LoginSuccessful();
        var json = message.ToJson();

        Assert.Contains("\"success\":true", json);
        Assert.Contains("\"code\":\"AUTH_003\"", json);
    }

    [Fact]
    public void ToJson_ShouldWorkWithoutCustomization()
    {
        Dispose();
        // 3. Zero config - just use it (defaults only)
        var msg = Msg.Auth.LoginFailed(); // Works without Configure()

        Assert.Contains("Authentication Failed", msg.Title);
        Assert.Equal("AUTH_001", msg.Code);
    }
}
