using System.Diagnostics;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.Core;
using RecurPixel.EasyMessages.Tests.Unit;
using Xunit;

namespace RecurPixel.EasyMessages.Tests.Integration;

public class PerformanceTests : UnitTestBase
{
    [Fact]
    public void MessageRegistry_Get_ShouldBeFast()
    {
        Dispose();
        // Warm up
        _ = MessageRegistry.Get("AUTH_001");
        // Measure
        var sw = Stopwatch.StartNew();
        for (int i = 0; i < 10000; i++)
        {
            _ = MessageRegistry.Get("AUTH_001");
        }
        sw.Stop();

        // Assert - Should complete 10k lookups quickly (allowing for CI variability)
        Assert.True(
            sw.ElapsedMilliseconds < 50,
            $"10k lookups took {sw.ElapsedMilliseconds}ms, expected < 100ms"
        );
    }

    [Fact]
    public void ToJson_ShouldBeFast()
    {
        Dispose();
        var message = MessageRegistry.Get("AUTH_001").WithData(new { UserId = 123 });

        // Warm up
        _ = message.ToJson();

        // Measure
        var sw = Stopwatch.StartNew();
        for (int i = 0; i < 1000; i++)
        {
            _ = message.ToJson();
        }
        sw.Stop();

        // Assert - Should complete 1k formatting in under 100ms
        Assert.True(
            sw.ElapsedMilliseconds < 100,
            $"1k JSON formatting took {sw.ElapsedMilliseconds}ms, expected < 100ms"
        );
    }
}
