using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.Core;
using Xunit;

namespace RecurPixel.EasyMessages.Tests.Unit;

[Collection("MessageRegistry")]
public class MessageExtensionsTests : UnitTestBase
{
    [Fact]
    public void WithData_ShouldAddData()
    {
        Dispose();
        // Arrange
        var message = MessageRegistry.Get("AUTH_001");
        var data = new { UserId = 123, Username = "john" };

        // Act
        var enriched = message.WithData(data);

        // Assert
        Assert.Null(message.Data); // Original unchanged
        Assert.NotNull(enriched.Data);
        Assert.Equal(data, enriched.Data);
    }

    [Fact]
    public void WithMetadata_ShouldAddMetadata()
    {
        Dispose();
        // Arrange
        var message = MessageRegistry.Get("AUTH_001");

        // Act
        var enriched = message.WithMetadata("userId", 123).WithMetadata("ipAddress", "192.168.1.1");

        // Assert
        Assert.Empty(message.Metadata); // Original unchanged
        Assert.Equal(2, enriched.Metadata.Count);
        Assert.Equal(123, enriched.Metadata["userId"]);
        Assert.Equal("192.168.1.1", enriched.Metadata["ipAddress"]);
    }

    [Fact]
    public void WithCorrelationId_ShouldSetCorrelationId()
    {
        Dispose();
        // Arrange
        var message = MessageRegistry.Get("AUTH_001");
        var correlationId = Guid.NewGuid().ToString();

        // Act
        var enriched = message.WithCorrelationId(correlationId);

        // Assert
        Assert.Null(message.CorrelationId); // Original unchanged
        Assert.Equal(correlationId, enriched.CorrelationId);
    }

    [Fact]
    public void WithStatusCode_ShouldOverrideStatusCode()
    {
        Dispose();
        // Arrange
        var message = MessageRegistry.Get("AUTH_001");

        // Act
        var enriched = message.WithStatusCode(429);

        // Assert
        Assert.Equal(401, message.HttpStatusCode); // Original unchanged
        Assert.Equal(429, enriched.HttpStatusCode);
    }

    [Fact]
    public void WithParams_ShouldSubstituteParameters()
    {
        Dispose();
        // Arrange
        var message = MessageRegistry.Get("CRUD_001");

        // Act
        var substituted = message.WithParams(new { resource = "User" });

        // Assert
        Assert.Contains("User has been created", substituted.Description);
    }

    [Fact]
    public void WithParams_ShouldBeCaseInsensitive()
    {
        Dispose();
        // Arrange
        var message = MessageRegistry.Get("VAL_002");

        // Act
        var substituted1 = message.WithParams(new { field = "Email" });
        var substituted2 = message.WithParams(new { Field = "Email" });
        var substituted3 = message.WithParams(new { FIELD = "Email" });

        // Assert
        Assert.Contains("The field 'Email' is required.", substituted1.Description);
        Assert.Contains("The field 'Email' is required.", substituted2.Description);
        Assert.Contains("The field 'Email' is required.", substituted3.Description);
    }

    [Fact]
    public void WithParams_ShouldHandleMissingParameters()
    {
        Dispose();
        // Arrange
        var message = MessageRegistry.Get("CRUD_001");

        // Act
        var substituted = message.WithParams(new { wrongParam = "Value" });

        // Assert - Placeholder should remain
        Assert.Contains("{resource}", substituted.Description);
    }

    [Fact]
    public void WithParams_ShouldHandleMultipleParameters()
    {
        Dispose();
        // Arrange
        var custom = new Dictionary<string, MessageTemplate>
        {
            ["TEST_001"] = new()
            {
                Type = MessageType.Success,
                Title = "Welcome {name}",
                Description = "Your role is {role} in {department}",
            },
        };
        MessageRegistry.LoadCustomMessages(custom);
        var message = MessageRegistry.Get("TEST_001");

        // Act
        var substituted = message.WithParams(
            new
            {
                name = "John",
                role = "Admin",
                department = "IT",
            }
        );

        // Assert
        Assert.Contains("Welcome John", substituted.Title);
        Assert.Contains("Your role is Admin in IT", substituted.Description);
    }

    [Fact]
    public void Chaining_ShouldWork()
    {
        Dispose();
        // Arrange
        var message = MessageRegistry.Get("AUTH_001");

        // Act
        var enriched = message
            .WithData(new { UserId = 123 })
            .WithMetadata("attempt", 3)
            .WithMetadata("ipAddress", "192.168.1.1")
            .WithCorrelationId(Guid.NewGuid().ToString())
            .WithStatusCode(429);

        // Assert
        Assert.NotNull(enriched.Data);
        Assert.Equal(2, enriched.Metadata.Count);
        Assert.NotNull(enriched.CorrelationId);
        Assert.Equal(429, enriched.HttpStatusCode);
    }
}
