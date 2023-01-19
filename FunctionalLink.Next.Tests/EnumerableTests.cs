using Xunit;

namespace FunctionalLink.Next.Tests;

public class EnumerableTests
{    
    [Fact]
    public void IterateExecutesAction()
    {
        var actual = "";
        
        "test".Enumerate()
            .Iterate(s => actual = s)
            .EvaluateAndIgnore();
        
        Assert.Equal("test", actual);
    }

    [Fact]
    public void EvaluatePreventsReevaluationOfAnEnumerable()
    {
        var actual = 0;
        var sut = "test".Enumerate()
            .Iterate(i => actual++)
            .Evaluate();
        
        sut.Min()!.Ignore();
        sut.Max()!.Ignore();
        sut.Last().Ignore(); 
        
        Assert.Equal(1, actual);
    }

    [Fact]
    public void EvaluateAndIgnoreForcesEvaluationOfAnEnumerable()
    {
        var actual = 0;
        
        "test".Enumerate()
            .Iterate(_ => actual++)
            .EvaluateAndIgnore();
        
        Assert.Equal(1, actual);
    }
}