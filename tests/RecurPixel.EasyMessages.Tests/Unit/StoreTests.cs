using RecurPixel.EasyMessages.Core;
using RecurPixel.EasyMessages.Storage;
using Xunit;

namespace RecurPixel.EasyMessages.Tests.Unit;

public class StoreTests : UnitTestBase
{
    [Fact]
    public async Task DictionaryStore_ShouldLoadMessages()
    {
        Dispose();
        // Arrange
        var messages = new Dictionary<string, MessageTemplate>
        {
            ["TEST_001"] = new()
            {
                Type = MessageType.Success,
                Title = "Test",
                Description = "Test message",
            },
        };
        var store = new DictionaryMessageStore(messages);

        // Act
        var loaded = await store.LoadAsync();

        // Assert
        Assert.Single(loaded);
        Assert.Equal("Test", loaded["TEST_001"].Title);
    }

    [Fact]
    public async Task CompositeStore_ShouldMergeStores()
    {
        Dispose();
        // Arrange
        var store1 = new DictionaryMessageStore(
            new Dictionary<string, MessageTemplate>
            {
                ["TEST_001"] = new() { Title = "Store 1", Description = "From 1" },
            }
        );

        var store2 = new DictionaryMessageStore(
            new Dictionary<string, MessageTemplate>
            {
                ["TEST_002"] = new() { Title = "Store 2", Description = "From 2" },
            }
        );

        var composite = new CompositeMessageStore(store1, store2);

        // Act
        var loaded = await composite.LoadAsync();

        // Assert
        Assert.Equal(2, loaded.Count);
        Assert.Contains("TEST_001", loaded.Keys);
        Assert.Contains("TEST_002", loaded.Keys);
    }

    [Fact]
    public async Task CompositeStore_LastStoreWins()
    {
        Dispose();
        // Arrange
        var store1 = new DictionaryMessageStore(
            new Dictionary<string, MessageTemplate>
            {
                ["TEST_001"] = new() { Title = "Store 1", Description = "From 1" },
            }
        );

        var store2 = new DictionaryMessageStore(
            new Dictionary<string, MessageTemplate>
            {
                ["TEST_001"] = new() { Title = "Store 2", Description = "From 2" },
            }
        );

        var composite = new CompositeMessageStore(store1, store2);

        // Act
        var loaded = await composite.LoadAsync();

        // Assert
        Assert.Single(loaded);
        Assert.Equal("Store 2", loaded["TEST_001"].Title);
    }
}
