using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.Core;
using RecurPixel.EasyMessages.Tests.Unit;
using Xunit;

namespace RecurPixel.EasyMessages.Tests.Integration;

[Collection("MessageRegistry")]
public class EndToEndTests : UnitTestBase
{
    [Fact]
    public void CompleteWorkflow_LoadCustomMessages_UseInMultipleFormats()
    {
        Dispose();
        // Arrange
        var custom = new Dictionary<string, MessageTemplate>
        {
            ["WORKFLOW_001"] = new()
            {
                Type = MessageType.Success,
                Title = "Workflow Complete",
                Description = "Process {processName} completed successfully",
            },
        };
        MessageRegistry.LoadCustomMessages(custom);

        // Act
        var message = Msg.Custom("WORKFLOW_001")
            .WithParams(new { processName = "DataImport" })
            .WithData(new { recordsProcessed = 1000 })
            .WithMetadata("duration", "5 minutes")
            .WithCorrelationId(Guid.NewGuid().ToString());

        var json = message.ToJson();
        var xml = message.ToXml();
        var text = message.ToPlainText();

        // Assert
        Assert.Contains("DataImport", json);
        Assert.Contains("DataImport", xml);
        Assert.Contains("DataImport", text);
        Assert.Contains("recordsProcessed", json);
    }

    [Fact]
    public void RealWorldScenario_UserRegistration()
    {
        Dispose();
        // Simulate user registration workflow
        var message = Msg
            .Crud.Created("User")
            .WithData(
                new
                {
                    UserId = 12345,
                    Email = "john@example.com",
                    RegistrationDate = DateTime.UtcNow,
                }
            )
            .WithMetadata("source", "WebApp")
            .WithMetadata("userAgent", "Mozilla/5.0")
            .WithCorrelationId("REG-2024-001");

        var json = message.ToJson();

        Assert.Contains("User has been created", json);
        Assert.Contains("john@example.com", json);
        Assert.Contains("REG-2024-001", json);
    }
}
