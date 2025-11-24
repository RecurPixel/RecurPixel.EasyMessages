using RecurPixel.EasyMessages.Core;
using Xunit;

namespace RecurPixel.EasyMessages.Tests.Unit;

public class MessageTests : UnitTestBase
{
    [Fact]
    public void Message_ShouldBeImmutable()
    {
        Dispose();
        // Arrange
        var message = new MessageTemplate
        {
            Type = MessageType.Success,
            Title = "Test",
            Description = "Test Description",
        }.ToMessage("TEST_001");

        // Act - try to modify (should create new instance)
        var modified = message with
        {
            Title = "Modified",
        };

        // Assert
        Assert.NotEqual(message.Title, modified.Title);
        Assert.Equal("Test", message.Title); // Original unchanged
    }

    [Fact]
    public void Message_ShouldAutoGenerateTimestamp()
    {
        Dispose();
        // Arrange & Act
        var message = new MessageTemplate
        {
            Type = MessageType.Info,
            Title = "Test",
            Description = "Test",
        }.ToMessage("TEST_001");

        // Assert
        Assert.True((DateTime.UtcNow - message.Timestamp).TotalSeconds < 1);
    }

    [Fact]
    public void Message_ShouldInitializeEmptyCollections()
    {
        Dispose();
        // Arrange & Act
        var message = new MessageTemplate
        {
            Type = MessageType.Info,
            Title = "Test",
            Description = "Test",
        }.ToMessage("TEST_001");

        // Assert
        Assert.NotNull(message.Metadata);
        Assert.Empty(message.Metadata);
    }

    [Theory]
    [InlineData(MessageType.Success)]
    [InlineData(MessageType.Info)]
    [InlineData(MessageType.Warning)]
    [InlineData(MessageType.Error)]
    [InlineData(MessageType.Critical)]
    public void Message_ShouldSupportAllMessageTypes(MessageType type)
    {
        Dispose();
        // Arrange & Act
        var message = new MessageTemplate
        {
            Type = type,
            Title = "Test",
            Description = "Test",
        }.ToMessage("TEST_001");

        // Assert
        Assert.Equal(type, message.Type);
    }
}
