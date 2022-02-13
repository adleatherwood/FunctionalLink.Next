using Xunit;
using static FunctionalLink.Next.GlobalLink;

namespace FunctionalLink.Next.Tests;

public class SuccessTests
{
    [Fact]
    public void SuccessIsConstructedWithGivenValue()
    {
        var actual = ((Result<int>) Result.Success(1))
            .Match(Self, failure => 0);
        
        Assert.Equal(1, actual);
    }
    
    [Fact]
    public void SuccessIsConstructedWithNoneValue()
    {
        var actual = ((Result<None>) Result.Success())
            .Match(
                success => 1, 
                failure => 0);
        
        Assert.Equal(1, actual);
    }
}