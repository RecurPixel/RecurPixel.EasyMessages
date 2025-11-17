using System.Reflection;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.Core;
using RecurPixel.EasyMessages.Core.Extensions;
using RecurPixel.EasyMessages.Exceptions;

namespace RecurPixel.EasyMessages.Tests;

public class SampleTest
{
    [Fact]
    public void Message_ShouldHaveTimestamp()
    {
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
    public void MessageTemplate_ShouldMapToMessage()
    {
        var template = new MessageTemplate
        {
            Type = MessageType.Error,
            Title = "Test",
            Description = "Test Description",
        };

        var message = template.ToMessage("TEST_001");

        Assert.Equal("TEST_001", message.Code);
        Assert.Equal(MessageType.Error, message.Type);
        Assert.Equal(400, message.HttpStatusCode);
    }

    [Fact]
    public void Registry_ShouldLoadDefaultMessages()
    {
        var message = MessageRegistry.Get("AUTH_001");
        Assert.Equal("AUTH_001", message.Code);
        Assert.Equal("Authentication Failed", message.Title);
    }

    [Fact]
    public void MessageCodes_Constants_ShouldMatchDefaultsJson()
    {
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
        Assert.Throws<MessageNotFoundException>(() => MessageRegistry.Get("INVALID_999"));
    }

    [Fact]
    public void WithData_ShouldAddData()
    {
        var message = MessageRegistry.Get("AUTH_003").WithData(new { UserId = 123 });

        Assert.NotNull(message.Data);
    }

    [Fact]
    public void WithParams_ShouldReplaceTemplateValues()
    {
        var message = MessageRegistry.Get("CRUD_001").WithParams(new { resource = "User" });

        Assert.Equal("User has been created.", message.Description);
    }

    [Fact]
    public void Msg_Auth_LoginFailed_ShouldWork()
    {
        var message = Msg.Auth.LoginFailed();
        Assert.Equal("AUTH_001", message.Code);
    }

    [Fact]
    public void Msg_Crud_Created_ShouldReplaceResource()
    {
        var message = Msg.Crud.Created("User");
        Assert.Contains("User", message.Description);
    }

    [Fact]
    public void Msg_Custom_ShouldAccessAnyCode()
    {
        var message = Msg.Custom("AUTH_001");
        Assert.Equal("AUTH_001", message.Code);
    }

    [Fact]
    public void ToJson_ShouldSerializeCorrectly()
    {
        var message = Msg.Auth.LoginSuccess();
        var json = message.ToJson();

        Assert.Contains("\"success\":true", json);
        Assert.Contains("\"code\":\"AUTH_003\"", json);
    }
}
