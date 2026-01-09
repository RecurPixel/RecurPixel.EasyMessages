using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Microsoft.AspNetCore.Mvc;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.AspNetCore;
using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages.Tests.Unit;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80)]
[MinColumn, MaxColumn, MeanColumn, MedianColumn, StdDevColumn, AllStatisticsColumn]
public class MessagePerformanceBenchmarks
{
    private Message _message = null!;
    private MessageTemplate _template = null!;
    private MessageTemplate _complexTemplate = null!;
    private Message _complexMessage = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Simple template for baseline tests
        _template = new MessageTemplate
        {
            Type = MessageType.Success,
            Title = "Test {name}",
            Description = "Testing {value}",
            HttpStatusCode = 200,
        };
        _message = _template.ToMessage("TEST_001");

        // Complex template with more parameters
        _complexTemplate = new MessageTemplate
        {
            Type = MessageType.Warning,
            Title = "Complex {operation} for {user}",
            Description = "Operation {operation} completed with status {status} at {timestamp}",
            HttpStatusCode = 201,
        };
        _complexMessage = _complexTemplate.ToMessage("TEST_COMPLEX");
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        // Cleanup if needed
    }

    [Benchmark(Baseline = true, Description = "Baseline: Convert message to API response")]
    public IActionResult ToApiResponse_Simple()
    {
        return _message.ToApiResponse();
    }

    [Benchmark(Description = "Convert complex message to API response")]
    public IActionResult ToApiResponse_Complex()
    {
        return _complexMessage.ToApiResponse();
    }

    [Benchmark(Description = "Simple message with parameters")]
    public Message WithParams_Simple()
    {
        return _message.WithParams(new { name = "John", value = "42" });
    }

    [Benchmark(Description = "Complex message with multiple parameters")]
    public Message WithParams_Complex()
    {
        return _complexMessage.WithParams(
            new
            {
                operation = "CreateUser",
                user = "admin@example.com",
                status = "Success",
                timestamp = DateTime.UtcNow,
            }
        );
    }

    [Benchmark(Description = "Add single metadata")]
    public Message WithMetadata_Single()
    {
        return _message.WithMetadata("key", "value");
    }

    [Benchmark(Description = "Add multiple metadata items")]
    public Message WithMetadata_Multiple()
    {
        var msg = _message;
        msg = msg.WithMetadata("correlation_id", Guid.NewGuid().ToString());
        msg = msg.WithMetadata("request_id", Guid.NewGuid().ToString());
        msg = msg.WithMetadata("user_id", "user123");
        return msg;
    }

    [Benchmark(Description = "Create template from scratch")]
    public Message CreateTemplate_Benchmark()
    {
        var template = new MessageTemplate
        {
            Type = MessageType.Error,
            Title = "Error {code}",
            Description = "An error occurred: {message}",
            HttpStatusCode = 500,
        };
        return template.ToMessage("ERROR_001");
    }

    [Benchmark(Description = "Chained operations")]
    public IActionResult ChainedOperations_Benchmark()
    {
        return _message
            .WithParams(new { name = "Test", value = "123" })
            .WithMetadata("source", "benchmark")
            .WithMetadata("version", "1.0")
            .ToApiResponse();
    }
}
