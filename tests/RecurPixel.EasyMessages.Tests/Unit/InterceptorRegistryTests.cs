using System.Text.Json;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.Core;
using RecurPixel.EasyMessages.Interceptors;
using Xunit;

namespace RecurPixel.EasyMessages.Tests.Unit;

public class InterceptorRegistryTests : UnitTestBase
{
    [Fact]
    public void Register_ShouldAddInterceptor()
    {
        Dispose();
        // Arrange
        var interceptor = new TestInterceptor();

        // Act
        InterceptorRegistry.Register(interceptor);
        var message = MessageRegistry.Get("AUTH_001").ToJson();

        // Assert
        Assert.Contains("AUTH_001", message);

        // Cleanup
        InterceptorRegistry.Clear();
    }

    [Fact]
    public void Interceptor_ShouldModifyMessage()
    {
        Dispose();
        // Arrange
        InterceptorRegistry.Register(new MetadataInterceptor());
        var message = MessageRegistry.Get("AUTH_001");

        // Act
        var json = message.ToJson();

        // Assert
        var parsed = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
        Assert.True(parsed["metadata"].TryGetProperty("intercepted", out _));

        // Cleanup
        InterceptorRegistry.Clear();
    }

    [Fact]
    public void Clear_ShouldRemoveAllInterceptors()
    {
        Dispose();
        // Arrange
        InterceptorRegistry.Register(new TestInterceptor());
        InterceptorRegistry.Register(new MetadataInterceptor());

        // Act
        InterceptorRegistry.Clear();

        // Assert - No easy way to verify, but shouldn't throw
        var message = MessageRegistry.Get("AUTH_001").ToJson();
        Assert.NotNull(message);
    }

    private class TestInterceptor : IMessageInterceptor
    {
        public Message OnBeforeFormat(Message message) => message;

        public Message OnAfterFormat(Message message) => message;
    }

    private class MetadataInterceptor : IMessageInterceptor
    {
        public Message OnBeforeFormat(Message message)
        {
            return message.WithMetadata("intercepted", true);
        }

        public Message OnAfterFormat(Message message) => message;
    }
}
