using Xunit;
using static FunctionalLink.Next.GlobalLink;

namespace FunctionalLink.Next.Tests;

public class UnitTest1
{
    // [Fact]
    // public void Test1()
    // {
    //     Result<int> Test()
    //     {
    //         // todo this explicit cast should be unnecessary
    //         var x = Result<int>.Success(1);
    //
    //         x.HasValue(out var value);
    //         x.HasFailure(out var failure);
    //         x.Match(
    //             Console.WriteLine,
    //             Console.WriteLine);
    //
    //         var y = x
    //             .Then(i => 2 + i);
    //         
    //         return y;
    //     }
    //
    //     var actual = Test().Match(
    //         s => s,
    //         f => -1);
    //     
    //     Assert.Equal(3, actual);
    // }
    //
    // [Fact]
    // public void ExceptionFormatting()
    // {
    //     Result<int> DoIt()
    //     {
    //         try
    //         {
    //             throw new Exception("This is a test.");
    //             //return Result<int,Exception>.Success(0);
    //         }
    //         catch (Exception e)
    //         {
    //             return Result<int, Exception>.Failure(e);
    //         }
    //     }
    //
    //     var sut = DoIt();
    //     var actual = DoIt().Match(
    //         success => success.ToString(),
    //         failure => failure);
    //
    //     Assert.Contains("This is a test.", actual);
    // }

    
}