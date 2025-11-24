using RecurPixel.EasyMessages.Core;
using Xunit;

namespace RecurPixel.EasyMessages.Tests.Unit;

public class UnitTestBase
{
    public UnitTestBase() => MessageRegistry.Reset();

    public void Dispose() => MessageRegistry.Reset();
}
