using Xunit;

namespace FunctionalLink.Next.Tests;

public class ObjectsTests
{
    [Fact]
    public void EnumerateCreatesASingleElementCollection()
    {
        var actual = "test".Enumerate()
            .Single();

        Assert.Equal("test", actual);
    }

    [Fact]
    public void IgnoreReturnsVoid()
    {
        "test".Ignore();
    }

    [Fact]
    public async Task AsyncIgnoreReturnsTask()
    {
        await Task.FromResult("test").Ignore();
    }
}