using Xunit;
using static FunctionalLink.Next.GlobalLink;

namespace FunctionalLink.Next.Tests;

public class FailureTests
{
    [Fact]
    public void FailureIsConstructedWithTheGivenValue()
    {
        var actual = ((Result<string>) Result.Failure("test"))
            .Match(Self, Self);
        
        Assert.Equal("test", actual);
    }
}