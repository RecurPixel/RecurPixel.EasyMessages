using RecurPixel.EasyMessages.Core;
using Xunit;

namespace RecurPixel.EasyMessages.Tests.Unit;

public class UnitTestBase
{
    // Note: avoid resetting the global MessageRegistry in the constructor because
    // xUnit constructs test classes in parallel; a constructor-level reset would
    // race with other tests. Tests should call `Dispose()` explicitly where needed.
    public UnitTestBase() { }

    public void Dispose() => MessageRegistry.Reset();
}
