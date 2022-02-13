using Xunit;
using static FunctionalLink.Next.GlobalLink;

namespace FunctionalLink.Next.Tests;

public class GlobalLinkTests
{
    [Fact]
    public void SelfReturnsGivenValue()
    {
        var actual = Self(1);
        
        Assert.Equal(1, actual);
    }

    [Fact]
    public void SomeConstructsOptionWithGivenValue()
    {
        var actual = Some(1)
            .ValueOr(0);
        
        Assert.Equal(1, actual);
    }

    [Fact]
    public void NoneConstructsOptionWithoutAValue()
    {
        var actual = ((Option<int>) None())
            .ValueOr(1);
        
        Assert.Equal(1, actual);
    }

    [Fact]
    public void MaybeConstructOptionInSomeStateWhenValueIsNotNull()
    {
        var actual = Maybe("test")
            .ValueOr(null);
        
        Assert.Equal("test", actual);
    }
    
    [Fact]
    public void MaybeConstructOptionInNoneStateWhenValueIsNull()
    {
        var actual = Maybe(default(string))
            .ValueOr("test");
        
        Assert.Equal("test", actual);
    }

    [Fact]
    public void SuccessConstructsResultWithGivenValue()
    {
        var actual = ((Result<int>) Success(1))
            .ValueOr(0);
        
        Assert.Equal(1, actual);
    }
    
    [Fact]
    public void SuccessConstructsResultWithNoValue()
    {
        var actual = ((Result<None>) Success())
            .Match(
                _ => 1,
                _ => 0);
        
        Assert.Equal(1, actual);
    }
    
    [Fact]
    public void FailureConstructsResultWithGivenValue()
    {
        var actual = ((Result<int>) Failure("test"))
            .HasFailure(out var found);
        
        Assert.True(actual);
        Assert.Equal("test", found);
    }
}