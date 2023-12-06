using Xunit;
using static FunctionalLink.Next.GlobalLink;

namespace FunctionalLink.Next.Tests;

#pragma warning disable 8620

public class ResultTests
{
    [Fact]
    public async Task LeftConstructorMatchesProperly()
    {
        var actual = await AsyncResult<int>.Success(1)
            .Match(
                value => value,
                other => -1);
        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task RightConstructorMatchesProperly()
    {
        var actual = await AsyncResult<int>.Failure("")
            .Match(
                value => -1,
                other => 1);
        Assert.Equal(1, actual);
    }

    [Fact]
    public void ImplicitConversionFromExceptionMatchesProperly()
    {
        var value = new Exception();
        var actual = ((Result<int>) value)
            .Match(
                value => -1,
                other => 1);
        Assert.Equal(1, actual);
    }

    [Fact]
    public void ImplicitConversionFromErrorMatchesProperly()
    {
        var value = new Error("");
        var actual = ((Result<int>) value)
            .Match(
                value => -1,
                other => 1);
        Assert.Equal(1, actual);
    }

    //========================================================================== Inspect

    [Fact]
    public async Task HasValueIsTrueOnValue()
    {        
        var actual = await AsyncResult<int>.Success(1)
            .HasSuccess();

        Assert.True(actual);
    }

    [Fact]
    public async Task HasValueIsFalseOnOther()
    {        
        var actual = await AsyncResult<int>.Failure("")
            .HasSuccess();

        Assert.False(actual);
    }

    [Fact]
    public async Task HasOtherIsTrueOnOther()
    {        
        var actual = await AsyncResult<int>.Failure("")
            .HasFailure();

        Assert.True(actual);
    }

    [Fact]
    public async Task HasValueIsFalseOnValue()
    {        
        var actual = await AsyncResult<int>.Success(1)
            .HasFailure();

        Assert.False(actual);
    }

    //========================================================================== Decompose 

    //-------------------------------------------------------------------------- Has

    [Fact]
    public void HasValueOutReturnsOnValue()
    {        
        Result<int>.Success(1)
            .HasSuccess(out var actual)
            .Ignore();

        Assert.Equal(1, actual);
    }

    [Fact]
    public void HasOtherOutReturnsOnOther()
    {        
        Result<int>.Failure("test")
            .HasFailure(out var actual)
            .Ignore();

        Assert.Equal("test", actual.Message);
    }

    [Fact]
    public void HasValue2OutReturnsOnValue()
    {        
        Result<int>.Success(1)
            .HasSuccess(out var actual, out var _)
            .Ignore();

        Assert.Equal(1, actual);
    }

    [Fact]
    public void HasValue2OutReturnsOnError()
    {        
        Result<int>.Failure("test")
            .HasSuccess(out var _, out var error)
            .Ignore();

        Assert.Equal("test", error.Message);
    }

    //-------------------------------------------------------------------------- Match

    [Fact]
    public async Task MatchActionEvaluatesOnValue()
    {        
        var actual = 0;
        await AsyncResult<int>.Success(1)
            .Match(
                value => { actual = value; },
                other => { actual = -1; });

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task MatchActionEvaluatesOnOther()
    {        
        var actual = 0;
        await AsyncResult<int>.Failure("")
            .Match(
                value => { actual = -1; },
                other => { actual = 1; });

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task MatchActionAsyncEvaluatesOnValue()
    {        
        var actual = 0;
        await AsyncResult<int>.Success(1)
            .Match(
                value => { actual = value; ; return Task.CompletedTask; },
                other => { actual = -1; ; return Task.CompletedTask; });

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task MatchActionAsyncEvaluatesOnOther()
    {        
        var actual = 0;
        await AsyncResult<int>.Failure("")
            .Match(
                value => { actual = -1; return Task.CompletedTask; },
                other => { actual = 1; ; return Task.CompletedTask; });

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task MatchFuncEvaluatesOnValue()
    {                
        var actual = await AsyncResult<int>.Success(1)
            .Match(
                value => value,
                other => -1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task MatchFuncEvaluatesOnOther()
    {                
        var actual = await AsyncResult<int>.Failure("")
            .Match(
                value => -1,
                other => 1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task MatchFuncEvaluationsCanBeAsync()
    {                
        var actual = await AsyncResult<int>.Success(1)
            .Match(
                value => value,
                other => -1);
                // value => Task.FromResult(value),
                // other => Task.FromResult(-1));

        Assert.Equal(1, actual);
    }

    //-------------------------------------------------------------------------- ValueOr

    [Fact]
    public async Task ValueOrIsValueOnValue()
    {                
        var actual = await AsyncResult<int>.Success(1)
            .ValueOr(-1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task ValueOrIsOtherOnOther()
    {                
        var actual = await AsyncResult<int>.Failure("")
            .ValueOr(1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task ValueOrAcceptsNullWithoutAmbiguity()
    {                
        var actual = await AsyncResult<string>.Failure("")
            .ValueOr(null);

        Assert.Null(actual);
    }

    //========================================================================== Compose Left

    //-------------------------------------------------------------------------- Then/Map

    [Fact]
    public async Task ThenMapEvaluatesOnValue()
    {                
        var actual = await AsyncResult<int>.Success(0)
            .Then(value => value + 1)
            .ValueOr(-1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task ThenMapDoesNotEvaluateOnOther()
    {                    
        var fired = false;
        var actual = await AsyncResult<int>.Failure("")
            .Then(value => { fired = true; return value + 1; })            
            .ValueOr(1);

        Assert.False(fired);
        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task ThenMapAsyncEvaluatesOnValue()
    {                
        var actual = await AsyncResult<int>.Success(0)
            .Then(value => Task.FromResult(value + 1))
            .ValueOr(-1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task ThenMapAsyncDoesNotEvaluateOnOther()
    {                    
        var fired = false;
        var actual = await AsyncResult<int>.Failure("")
            .Then(value => { fired = true; return Task.FromResult(value + 1); })  
            .ValueOr(1);

        Assert.False(fired);
        Assert.Equal(1, actual);
    }

    //-------------------------------------------------------------------------- Then/Bind

    [Fact]
    public async Task ThenBindEvaluatesOnValue()
    {                
        var actual = await AsyncResult<int>.Success(0)
            .Then(value => Result<int>.Success(value + 1))
            .ValueOr(-1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task ThenBindDoesNotEvaluateOnOther()
    {                    
        var fired = false;
        var actual = await AsyncResult<int>.Failure("")
            .Then(value => { fired = true; return Result<int>.Success(value + 1); })            
            .ValueOr(1);

        Assert.False(fired);
        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task ThenBindAsyncEvaluatesOnValue()
    {                
        var actual = await AsyncResult<int>.Success(0)
            .Then(value => AsyncResult<int>.Success(value + 1))
            .ValueOr(-1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task ThenBindAsyncDoesNotEvaluateOnOther()
    {                    
        var fired = false;
        var actual = await AsyncResult<int>.Failure("")
            .Then(value => { fired = true; return AsyncResult<int>.Success(value + 1); })  
            .ValueOr(1);

        Assert.False(fired);
        Assert.Equal(1, actual);
    }

    //-------------------------------------------------------------------------- Then/Void

    [Fact]
    public async Task ThenVoidEvaluatesOnValue()
    {                
        var fired = false;
        await AsyncResult<int>.Success(0)
            .Then(value => { fired = true; })
            .Ignore();

        Assert.True(fired);
    }

    [Fact]
    public async Task ThenVoidDoesNotEvaluateOnOther()
    {                    
        var fired = false;
        await AsyncResult<int>.Failure("")
            .Then(value => { fired = true; })
            .Ignore();

        Assert.False(fired);        
    }

    [Fact]
    public async Task ThenVoidAsyncEvaluatesOnValue()
    {   
        var fired = false;             
        await AsyncResult<int>.Success(0)
            .Then(value => { fired = true; return Task.CompletedTask; })
            .Ignore();

        Assert.True(fired);
    }

    [Fact]
    public async Task ThenVoidAsyncDoesNotEvaluateOnOther()
    {                    
        var fired = false;
        await AsyncResult<int>.Failure("")
            .Then(value => { fired = true; return Task.CompletedTask; })
            .Ignore();

        Assert.False(fired);        
    }

    //========================================================================== Compose Right

    //-------------------------------------------------------------------------- Else/Map

    [Fact]
    public async Task ElseMapEvaluatesOnOther()
    {                
        var actual = await AsyncResult<int>.Failure("")
            .Else(value => 1)
            .ValueOr(-1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task ElseMapDoesNotEvaluateOnValue()
    {                    
        var fired = false;
        await AsyncResult<int>.Success(-1)
            .Else(value => { fired = true; return 1; })            
            .Ignore();

        Assert.False(fired);        
    }

    [Fact]
    public async Task ElseMapAsyncEvaluatesOnOther()
    {                
        var actual = await AsyncResult<int>.Failure("")
            .Else(value => Task.FromResult(1))
            .ValueOr(-1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task ElseMapAsyncDoesNotEvaluateOnValue()
    {                    
        var fired = false;
        var actual = await AsyncResult<int>.Success(1)
            .Else(value => { fired = true; return Task.FromResult(-1); })  
            .ValueOr(-1);

        Assert.False(fired);
        Assert.Equal(1, actual);
    }    

    //-------------------------------------------------------------------------- Else/Bind

    [Fact]
    public async Task ElseBindEvaluatesOnOther()
    {                
        var actual = await AsyncResult<int>.Failure("")
            .Else(value => Result<int>.Success(1))
            .ValueOr(-1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task ElseBindDoesNotEvaluateOnValue()
    {                    
        var fired = false;
        await AsyncResult<int>.Success(-1)
            .Else(value => { fired = true; return Result<int>.Success(1); })            
            .Ignore();

        Assert.False(fired);        
    }

    [Fact]
    public async Task ElseBindAsyncEvaluatesOnOther()
    {                
        var actual = await AsyncResult<int>.Failure("")
            .Else(value => AsyncResult<int>.Success(1))
            .ValueOr(-1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task ElseBindAsyncDoesNotEvaluateOnValue()
    {                    
        var fired = false;
        var actual = await AsyncResult<int>.Success(1)
            .Else(value => { fired = true; return AsyncResult<int>.Success(-1); })  
            .ValueOr(-1);

        Assert.False(fired);
        Assert.Equal(1, actual);
    }    

    //-------------------------------------------------------------------------- Else/Void

    [Fact]
    public async Task ElseVoidEvaluatesOnOther()
    {                
        var fired = false;
        await AsyncResult<int>.Failure("")
            .Else(value => { fired = true; });

        Assert.True(fired);
    }

    [Fact]
    public async Task ElseVoidDoesNotEvaluateOnValue()
    {                    
        var fired = false;
        await AsyncResult<int>.Success(-1)
            .Else(value => { fired = true; });

        Assert.False(fired);        
    }

    [Fact]
    public async Task ElseVoidAsyncEvaluatesOnOther()
    {   
        var fired = false;             
        await AsyncResult<int>.Failure("")
            .Else(value => { fired = true; return Task.CompletedTask; });

        Assert.True(fired);
    }

    [Fact]
    public async Task ElseVoidAsyncDoesNotEvaluateOnValue()
    {                    
        var fired = false;
        await AsyncResult<int>.Success(-1)
            .Else(value => { fired = true; return Task.CompletedTask; });

        Assert.False(fired);        
    }

    //========================================================================== Adapter Left

    //-------------------------------------------------------------------------- Filter

    [Fact]
    public async Task FilterReturnsStaticOtherOnFalse()
    {
        var actual = await AsyncResult<int>.Success(3)
            .Filter(value => value % 2 == 0, "not even")
            .Match(Self, _ => 1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task FilterReturnsFuncOtherOnFalse()
    {
        var actual = await AsyncResult<int>.Success(3)
            .Filter(value => value % 2 == 0, value => $"{value} is not even")
            .Match(Self, _ => 1);

        Assert.Equal(1, actual);
    }

    //========================================================================== Combinator Left

    //-------------------------------------------------------------------------- Or

    [Fact]
    public async Task OrStaticReturnsOriginalOnValue()
    {
        var original = AsyncResult<int>.Success(1);
        var alternate = Result<int>.Success(2);

        var actual = await original.Or(alternate)
            .ValueOr(-1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task OrStaticReturnsAlternateOnOther()
    {
        var original = AsyncResult<int>.Failure("");
        var alternate = Result<int>.Success(2);

        var actual = await original.Or(alternate)
            .ValueOr(-1);

        Assert.Equal(2, actual);
    }

    [Fact]
    public async Task OrFuncReturnsOriginalOnValue()
    {
        var original = AsyncResult<int>.Success(1);
        var alternate = Result<int>.Success(2);

        var actual = await original.Or(() => alternate)
            .ValueOr(-1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task OrFuncReturnsAlternateOnOther()
    {
        var original = AsyncResult<int>.Failure("");
        var alternate = Result<int>.Success(2);

        var actual = await original.Or(() => alternate)
            .ValueOr(-1);

        Assert.Equal(2, actual);
    }

    [Fact]
    public async Task OrFuncTaskReturnsOriginalOnValue()
    {
        var original = AsyncResult<int>.Success(1);
        var alternate = Result<int>.Success(2);

        var actual = await original.Or(() => Task.FromResult(alternate))
            .ValueOr(-1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task OrFuncTaskReturnsAlternateOnOther()
    {
        var original = AsyncResult<int>.Failure("");
        var alternate = Result<int>.Success(2);

        var actual = await original.Or(() => Task.FromResult(alternate))
            .ValueOr(-1);

        Assert.Equal(2, actual);
    }

    //-------------------------------------------------------------------------- And

    [Fact]
    public async Task AndStaticReturnsSelectedOnSuccess()
    {
        var a = AsyncResult<int>.Success(1);
        var b = Result<int>.Success(2);

        var actual = await a.And(b, (a,b) => a + b)
            .ValueOr(-1);

        Assert.Equal(3, actual);
    }

    [Fact]
    public async Task AndStaticReturnsOtherOnAOther()
    {
        var a = AsyncResult<int>.Failure("");
        var b = Result<int>.Success(2);

        var actual = await a.And(b, (a,b) => a + b)
            .Match(Self, _ => 1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task AndStaticReturnsOtherOnBOther()
    {
        var a = AsyncResult<int>.Success(1);
        var b = Result<int>.Failure("");

        var actual = await a.And(b, (a,b) => a + b)
            .Match(Self, _ => 2);

        Assert.Equal(2, actual);
    }

    [Fact]
    public async Task AndFuncReturnsSelectedOnSuccess()
    {
        var a = AsyncResult<int>.Success(1);
        var b = Result<int>.Success(2);

        var actual = await a.And(() => b, (a,b) => a + b)
            .ValueOr(-1);

        Assert.Equal(3, actual);
    }

    [Fact]
    public async Task AndFuncStaticReturnsOtherOnAOther()
    {
        var a = AsyncResult<int>.Failure("");
        var b = Result<int>.Success(2);

        var actual = await a.And(() => b, (a,b) => a + b)
            .Match(Self, _ => 1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task AndFuncStaticReturnsOtherOnBOther()
    {
        var a = AsyncResult<int>.Success(1);
        var b = Result<int>.Failure("");

        var actual = await a.And(() => b, (a,b) => a + b)
            .Match(Self, _ => 2);

        Assert.Equal(2, actual);
    }

    [Fact]
    public async Task AndFuncAsyncReturnsSelectedOnSuccess()
    {
        var a = AsyncResult<int>.Success(1);
        var b = AsyncResult<int>.Success(2);

        var actual = await a.And(() => b, (a,b) => a + b)
            .ValueOr(-1);

        Assert.Equal(3, actual);
    }

    [Fact]
    public async Task AndFuncAsyncStaticReturnsOtherOnAOther()
    {
        var a = AsyncResult<int>.Failure("");
        var b = AsyncResult<int>.Success(2);

        var actual = await a.And(() => b, (a,b) => a + b)
            .Match(Self, _ => 1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task AndFuncAsyncStaticReturnsOtherOnBOther()
    {
        var a = AsyncResult<int>.Success(1);
        var b = AsyncResult<int>.Failure("");

        var actual = await a.And(() => b, (a,b) => a + b)
            .Match(Self, _ => 2);

        Assert.Equal(2, actual);
    }

    //========================================================================== Factory

    [Fact]
    public void LeftFactoryInitializesLeft()
    {
        var actual = Result.Success(1)
            .HasSuccess();

        Assert.True(actual);
    }

    [Fact]
    public void RightFactoryInitializesRight()
    {
        var actual = ((Result<int>) Result.Failure(""))
            .HasSuccess();

        Assert.False(actual);
    }

    [Fact]
    public void RightExceptionFactoryInitializesRight()
    {
        var actual = ((Result<int>) Result.Failure(new Exception("")))
            .HasSuccess();

        Assert.False(actual);
    }

    [Fact]
    public void RightMessageAndExceptionFactoryInitializesRight()
    {
        var actual = ((Result<int>) Result.Failure("abc", new Exception("def")))
            .HasSuccess();

        Assert.False(actual);
    }

    //===

    [Fact]
    public void LeftFactoryInitializesLeft1()
    {
        var actual = Result.Success()
            .HasSuccess();

        Assert.True(actual);
    }

    [Fact]
    public void RightFactoryInitializesRight1()
    {
        var actual = ((Result) Result.Failure(""))
            .HasSuccess();

        Assert.False(actual);
    }

    [Fact]
    public void RightExceptionFactoryInitializesRight1()
    {
        var actual = ((Result) Result.Failure(new Exception("")))
            .HasSuccess();

        Assert.False(actual);
    }

    [Fact]
    public void RightMessageAndExceptionFactoryInitializesRight1()
    {
        var actual = ((Result) Result.Failure("abc", new Exception("def")))
            .HasSuccess();

        Assert.False(actual);
    }


    //========================================================================== Test Fixture

    private class AsyncResult<A>
    {
        public static Task<Result<A>> Success(A value) =>
            Task.FromResult(Result<A>.Success(value));

        public static Task<Result<A>> Failure(string value) =>
            Task.FromResult(Result<A>.Failure(value));
    }
}
