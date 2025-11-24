using RecurPixel.EasyMessages.Core;
using Xunit;

namespace RecurPixel.EasyMessages.Tests.Unit;

public class MessageTemplateTests : UnitTestBase
{
    // We are not allowing default HttpStatusCode mapping tests for now.
    // [Theory]
    // [InlineData(MessageType.Success, 200)]
    // [InlineData(MessageType.Info, 200)]
    // [InlineData(MessageType.Warning, 200)]
    // [InlineData(MessageType.Error, 400)]
    // [InlineData(MessageType.Critical, 500)]
    // public void ToMessage_ShouldMapDefaultHttpStatusCode(MessageType type, int expectedStatus)
    // {
    //     // Arrange
    //     var template = new MessageTemplate
    //     {
    //         Type = type,
    //         Title = "Test",
    //         Description = "Test",
    //     };

    //     // Act
    //     var message = template.ToMessage("TEST_001");

    //     // Assert
    //     Assert.Equal(expectedStatus, message.HttpStatusCode);
    // }

    [Fact]
    public void ToMessage_ShouldUseExplicitHttpStatusCode()
    {
        Dispose();
        // Arrange
        var template = new MessageTemplate
        {
            Type = MessageType.Error,
            Title = "Test",
            Description = "Test",
            HttpStatusCode = 418, // I'm a teapot
        };

        // Act
        var message = template.ToMessage("TEST_001");

        // Assert
        Assert.Equal(418, message.HttpStatusCode);
    }

    [Fact]
    public void ToMessage_ShouldSetCodeCorrectly()
    {
        Dispose();
        // Arrange
        var template = new MessageTemplate
        {
            Type = MessageType.Success,
            Title = "Test",
            Description = "Test",
        };

        // Act
        var message = template.ToMessage("CUSTOM_123");

        // Assert
        Assert.Equal("CUSTOM_123", message.Code);
    }
}
