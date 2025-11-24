using System.Text.Json;
using System.Xml.Linq;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.Core;
using Xunit;

namespace RecurPixel.EasyMessages.Tests.Unit;

public class FormatterTests : UnitTestBase
{
    [Fact]
    public void ToJson_ShouldProduceValidJson()
    {
        Dispose();
        // Arrange
        var message = MessageRegistry.Get("AUTH_001").WithData(new { UserId = 123 });

        // Act
        var json = message.ToJson();

        // Assert
        Assert.Contains("\"code\":\"AUTH_001\"", json);
        Assert.Contains("\"success\":false", json);
        Assert.Contains("\"type\":\"error\"", json);
        Assert.NotNull(JsonSerializer.Deserialize<JsonDocument>(json)); // Valid JSON
    }

    [Fact]
    public void ToJsonObject_ShouldReturnDictionary()
    {
        Dispose();
        // Arrange
        var message = MessageRegistry.Get("AUTH_001");

        // Act
        var obj = message.ToJsonObject();

        // Assert
        Assert.NotNull(obj);
        var dict = obj as IDictionary<string, object>;
        Assert.NotNull(dict);
        Assert.True(dict.ContainsKey("code"));
        Assert.True(dict.ContainsKey("success"));
    }

    [Fact]
    public void ToXml_ShouldProduceValidXml()
    {
        Dispose();
        // Arrange
        var message = MessageRegistry.Get("AUTH_001");

        // Act
        var xml = message.ToXml();

        // Assert
        var doc = XDocument.Parse(xml); // Should not throw
        Assert.Contains("type=\"error\"", xml);
        Assert.Contains("AUTH_001", xml);
    }

    [Fact]
    public void ToXmlDocument_ShouldReturnXDocument()
    {
        Dispose();
        // Arrange
        var message = MessageRegistry.Get("AUTH_001");

        // Act
        var doc = message.ToXmlDocument();

        // Assert
        Assert.NotNull(doc);
        Assert.NotNull(doc.Root);
        Assert.Equal("message", doc.Root.Name.LocalName);
    }

    [Fact]
    public void ToPlainText_ShouldProduceReadableText()
    {
        Dispose();
        // Arrange
        var message = MessageRegistry.Get("AUTH_001");

        // Act
        var text = message.ToPlainText();

        // Assert
        Assert.Contains("Authentication Failed", text);
        Assert.Contains("AUTH_001", text);
        Assert.Contains("Invalid username or password", text);
    }

    [Theory]
    [InlineData(MessageType.Success, true)]
    [InlineData(MessageType.Info, true)]
    [InlineData(MessageType.Warning, false)]
    [InlineData(MessageType.Error, false)]
    [InlineData(MessageType.Critical, false)]
    public void ToJson_ShouldSetSuccessFlag(MessageType type, bool expectedSuccess)
    {
        Dispose();
        // Arrange
        var template = new MessageTemplate
        {
            Type = type,
            Title = "Test",
            Description = "Test",
        };
        var message = template.ToMessage("TEST_001");

        // Act
        var json = message.ToJson();

        // Assert
        var expectedValue = expectedSuccess ? "true" : "false";
        Assert.Contains($"\"success\":{expectedValue}", json);
    }
}
