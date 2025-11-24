using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.Core;
using RecurPixel.EasyMessages.Exceptions;
using RecurPixel.EasyMessages.Storage;
using Xunit;

namespace RecurPixel.EasyMessages.Tests.Unit;

[Collection("MessageRegistry")]
public class MessageRegistryTests : UnitTestBase
{
    [Fact]
    public void Get_ShouldReturnBuiltInMessage()
    {
        Dispose();
        // Act
        var message = MessageRegistry.Get("AUTH_001");

        // Assert
        Assert.Equal("AUTH_001", message.Code);
        Assert.Equal("Authentication Failed", message.Title);
        Assert.Equal(MessageType.Error, message.Type);
        Assert.Equal(401, message.HttpStatusCode);
    }

    [Fact]
    public void Get_ShouldThrowForInvalidCode()
    {
        Dispose();
        // Act & Assert
        var exception = Assert.Throws<MessageNotFoundException>(() =>
            MessageRegistry.Get("INVALID_999")
        );

        Assert.Contains("INVALID_999", exception.Message);
        Assert.Contains("not found", exception.Message.ToLower());
    }

    [Fact]
    public void GetAllCodes_ShouldReturnAllBuiltInCodes()
    {
        Dispose();
        // Act
        var codes = MessageRegistry.GetAllCodes().ToList();

        // Assert
        Assert.NotEmpty(codes);
        Assert.Contains("AUTH_001", codes);
        Assert.Contains("CRUD_001", codes);
        Assert.Contains("VAL_001", codes);
        Assert.Contains("SYS_001", codes);
    }

    [Fact]
    public void LoadCustomMessages_ShouldOverrideDefaults()
    {
        Dispose();
        // Arrange
        var custom = new Dictionary<string, MessageTemplate>
        {
            ["AUTH_001"] = new()
            {
                Type = MessageType.Error,
                Title = "Custom Login Failed",
                Description = "Custom description",
                HttpStatusCode = 401,
            },
        };

        // Act
        MessageRegistry.LoadCustomMessages(custom);
        var message = MessageRegistry.Get("AUTH_001");

        // Assert
        Assert.Equal("Custom Login Failed", message.Title);
        Assert.Equal("Custom description", message.Description);

        MessageRegistry.Reset();
    }

    [Fact]
    public void LoadCustomMessages_ShouldAddNewMessages()
    {
        Dispose();
        // Arrange
        var custom = new Dictionary<string, MessageTemplate>
        {
            ["CUSTOM_001"] = new()
            {
                Type = MessageType.Success,
                Title = "Custom Success",
                Description = "Custom success message",
                HttpStatusCode = 200,
            },
        };

        // Act
        MessageRegistry.LoadCustomMessages(custom);
        var message = MessageRegistry.Get("CUSTOM_001");

        // Assert
        Assert.Equal("CUSTOM_001", message.Code);
        Assert.Equal("Custom Success", message.Title);
    }

    [Fact]
    public void Configure_ShouldWorkWithDictionaryStore()
    {
        Dispose();
        // Arrange
        var custom = new Dictionary<string, MessageTemplate>
        {
            ["TEST_001"] = new()
            {
                Type = MessageType.Info,
                Title = "Test Message",
                Description = "Test description",
            },
        };
        var store = new DictionaryMessageStore(custom);

        // Act
        MessageRegistry.Configure(store);
        var message = MessageRegistry.Get("TEST_001");

        // Assert
        Assert.Equal("Test Message", message.Title);
    }

    [Fact]
    public void Configure_ShouldWorkWithCompositeStore()
    {
        Dispose();
        // Arrange
        var store1 = new DictionaryMessageStore(
            new Dictionary<string, MessageTemplate>
            {
                ["TEST_001"] = new()
                {
                    Type = MessageType.Info,
                    Title = "Store 1 Message",
                    Description = "From store 1",
                },
            }
        );

        var store2 = new DictionaryMessageStore(
            new Dictionary<string, MessageTemplate>
            {
                ["TEST_001"] = new()
                {
                    Type = MessageType.Info,
                    Title = "Store 2 Message",
                    Description = "From store 2",
                },
            }
        );

        var composite = new CompositeMessageStore(store1, store2);

        // Act
        MessageRegistry.Configure(composite);
        var message = MessageRegistry.Get("TEST_001");

        // Assert - Last store wins
        Assert.Equal("Store 2 Message", message.Title);
    }
}
