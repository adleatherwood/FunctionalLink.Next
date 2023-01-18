using Xunit;

namespace FunctionalLink.Next.Tests;

public class SharedTests
{
    [Fact]
    public void ValueInitializesInternalValue()
    {
        var actual = new Value<int>(1);
        Assert.Equal(1, actual.Value_);
    }

    [Fact]
    public void OtherInitializesInternalValue()
    {
        var actual = new Other<int>(1);
        Assert.Equal(1, actual.Value);
    }

    [Fact]
    public void ErrorInitializesInternalMessage()
    {
        var actual = new Error("test");
        Assert.Equal("test", actual.Message);
    }

    [Fact]
    public void ErrorInitializesInternalException()
    {
        var exn = new Exception("test");
        var error = new Error(exn);
        var hasExn = error.HasException(out var found);

        Assert.Equal("test", error.Message);
        Assert.True(error.HasException());
        Assert.True(hasExn);
        Assert.Equal(exn, found);
    }

    [Fact]
    public void ErrorImplicitlyConvertsFromString()
    {
        var actual = (Error) "test";
        Assert.Equal("test", actual.Message);
    }

    [Fact]
    public void ErrorImplicitlyConvertsFromException()
    {
        var actual = (Error) new Exception("test");
        Assert.Equal("test", actual.Message);
    }
}
