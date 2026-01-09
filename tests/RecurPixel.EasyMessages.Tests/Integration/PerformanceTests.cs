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
        // Based on benchmarks: API response ~119ns, registry get should be sub-microsecond
        // 10k operations at ~1-2 microseconds each = 10-20ms typical
        Assert.True(
            sw.ElapsedMilliseconds < 100,
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

        // Assert - Should complete 1k formatting operations quickly
        // Based on benchmarks: Complex operations ~3 microseconds each
        // 1k operations at ~3 microseconds each = ~3ms typical
        Assert.True(
            sw.ElapsedMilliseconds < 200,
            $"1k JSON formatting took {sw.ElapsedMilliseconds}ms, expected < 200ms"
        );
    }
}
