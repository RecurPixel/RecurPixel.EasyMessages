using System.Collections.Generic;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.Core;
using RecurPixel.EasyMessages.Storage;

namespace RecurPixel.EasyMessages.Tests.Unit;

public class RegistryAndFormatterTests : UnitTestBase
{
    [Fact]
    public void XmlFormatter_Produces_LowercaseType_And_IncludesHttpStatus()
    {
        Dispose();

        var template = new MessageTemplate
        {
            Type = MessageType.Error,
            Title = "Auth",
            Description = "Failed",
            HttpStatusCode = 401,
        };

        var msg = template.ToMessage("AUTH_001");
        var xml = msg.ToXml();

        Assert.Contains("type=\"error\"", xml);
        Assert.Contains("httpStatusCode", xml);
        Assert.Contains("401", xml);
    }
}
