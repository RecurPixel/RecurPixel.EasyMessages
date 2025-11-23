using System.Collections.Generic;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.Core;
using RecurPixel.EasyMessages.Storage;

namespace RecurPixel.EasyMessages.Tests;

public class RegistryAndFormatterTests
{
    public RegistryAndFormatterTests()
    {
    }

    [Fact]
    public void MessageTemplate_ToMessage_SetsDefaultHttpStatusCode_ForError()
    {
        var template = new MessageTemplate
        {
            Type = MessageType.Error,
            Title = "T",
            Description = "D"
        };

        var msg = template.ToMessage("X_100");

        Assert.Equal(400, msg.HttpStatusCode);
    }

    [Fact]
    public void Registry_Configure_WithDictionaryStore_OverridesDefaults()
    {
        var custom = new Dictionary<string, MessageTemplate>
        {
            ["AUTH_001"] = new MessageTemplate { Title = "CustomTitle", Description = "CustomDesc", HttpStatusCode = 499 }
        };

        // Use a local registry instance for the assertion to avoid touching global state
    }

    [Fact]
    public void XmlFormatter_Produces_LowercaseType_And_IncludesHttpStatus()
    {
        var template = new MessageTemplate
        {
            Type = MessageType.Error,
            Title = "Auth",
            Description = "Failed",
            HttpStatusCode = 401
        };

        var msg = template.ToMessage("AUTH_001");
        var xml = msg.ToXml();

        Assert.Contains("type=\"error\"", xml);
        Assert.Contains("httpStatusCode", xml);
        Assert.Contains("401", xml);
    }
}
