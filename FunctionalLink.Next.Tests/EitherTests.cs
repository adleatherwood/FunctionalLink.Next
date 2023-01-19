using Xunit;
using static FunctionalLink.Next.GlobalLink;

namespace FunctionalLink.Next.Tests;

#pragma warning disable 8620

public class EitherTests
{
    [Fact]
    public async Task LeftConstructorMatchesProperly()
    {
        var actual = await AsyncEither<int,int>.Value(1)
            .Match(
                value => value,
                other => -1);
        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task RightConstructorMatchesProperly()
    {
        var actual = await AsyncEither<int,int>.Other(1)
            .Match(
                value => -1,
                other => other);
        Assert.Equal(1, actual);
    }

    [Fact]
    public void ImplicitConversionFromLeftMatchesProperly()
    {
        var value = new Value<int>(1);
        var actual = ((Either<int,int>) value)
            .Match(
                value => value,
                other => -1);
        Assert.Equal(1, actual);
    }

    [Fact]
    public void ImplicitConversionFromRightMatchesProperly()
    {
        var value = new Other<int>(1);
        var actual = ((Either<int,int>) value)
            .Match(
                value => -1,
                other => other);
        Assert.Equal(1, actual);
    }

    //========================================================================== Inspect

    [Fact]
    public async Task HasValueIsTrueOnValue()
    {        
        var actual = await AsyncEither<int,int>.Value(1)
            .HasValue();

        Assert.True(actual);
    }

    [Fact]
    public async Task HasValueIsFalseOnOther()
    {        
        var actual = await AsyncEither<int,int>.Other(1)
            .HasValue();

        Assert.False(actual);
    }

    [Fact]
    public async Task HasOtherIsTrueOnOther()
    {        
        var actual = await AsyncEither<int,int>.Other(1)
            .HasOther();

        Assert.True(actual);
    }

    [Fact]
    public async Task HasValueIsFalseOnValue()
    {        
        var actual = await AsyncEither<int,int>.Value(1)
            .HasOther();

        Assert.False(actual);
    }

    //========================================================================== Decompose 

    //-------------------------------------------------------------------------- Has

    [Fact]
    public void HasValueOutReturnsOnValue()
    {        
        Either<int,int>.Value(1)
            .HasValue(out var actual)
            .Ignore();

        Assert.Equal(1, actual);
    }

    [Fact]
    public void HasOtherOutReturnsOnOther()
    {        
        Either<int,int>.Other(1)
            .HasOther(out var actual)
            .Ignore();

        Assert.Equal(1, actual);
    }

    //-------------------------------------------------------------------------- Match

    [Fact]
    public async Task MatchActionEvaluatesOnValue()
    {        
        var actual = 0;
        await AsyncEither<int,int>.Value(1)
            .Match(
                value => { actual = value; },
                other => { actual = -1; });

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task MatchActionEvaluatesOnOther()
    {        
        var actual = 0;
        await AsyncEither<int,int>.Other(1)
            .Match(
                value => { actual = -1; },
                other => { actual = other; });

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task MatchActionAsyncEvaluatesOnValue()
    {        
        var actual = 0;
        await AsyncEither<int,int>.Value(1)
            .Match(
                value => { actual = value; ; return Task.CompletedTask; },
                other => { actual = -1; ; return Task.CompletedTask; });

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task MatchActionAsyncEvaluatesOnOther()
    {        
        var actual = 0;
        await AsyncEither<int,int>.Other(1)
            .Match(
                value => { actual = -1; return Task.CompletedTask; },
                other => { actual = other; ; return Task.CompletedTask; });

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task MatchFuncEvaluatesOnValue()
    {                
        var actual = await AsyncEither<int,int>.Value(1)
            .Match(
                value => value,
                other => -1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task MatchFuncEvaluatesOnOther()
    {                
        var actual = await AsyncEither<int,int>.Other(1)
            .Match(
                value => -1,
                other => 1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task MatchFuncEvaluationsCanBeAsync()
    {                
        var actual = await AsyncEither<int,int>.Value(1)
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
        var actual = await AsyncEither<int,int>.Value(1)
            .ValueOr(-1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task ValueOrIsOtherOnOther()
    {                
        var actual = await AsyncEither<int,int>.Other(-1)
            .ValueOr(1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task ValueOrAcceptsNullWithoutAmbiguity()
    {                
        var actual = await AsyncEither<string,int>.Other(1)
            .ValueOr(null);

        Assert.Null(actual);
    }

    //========================================================================== Compose Left

    //-------------------------------------------------------------------------- Then/Map

    [Fact]
    public async Task ThenMapEvaluatesOnValue()
    {                
        var actual = await AsyncEither<int,int>.Value(0)
            .Then(value => value + 1)
            .ValueOr(-1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task ThenMapDoesNotEvaluateOnOther()
    {                    
        var fired = false;
        var actual = await AsyncEither<int,int>.Other(-1)
            .Then(value => { fired = true; return value + 1; })            
            .ValueOr(1);

        Assert.False(fired);
        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task ThenMapAsyncEvaluatesOnValue()
    {                
        var actual = await AsyncEither<int,int>.Value(0)
            .Then(value => Task.FromResult(value + 1))
            .ValueOr(-1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task ThenMapAsyncDoesNotEvaluateOnOther()
    {                    
        var fired = false;
        var actual = await AsyncEither<int,int>.Other(-1)
            .Then(value => { fired = true; return Task.FromResult(value + 1); })  
            .ValueOr(1);

        Assert.False(fired);
        Assert.Equal(1, actual);
    }

    //-------------------------------------------------------------------------- Then/Bind

    [Fact]
    public async Task ThenBindEvaluatesOnValue()
    {                
        var actual = await AsyncEither<int,int>.Value(0)
            .Then(value => Either<int,int>.Value(value + 1))
            .ValueOr(-1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task ThenBindDoesNotEvaluateOnOther()
    {                    
        var fired = false;
        var actual = await AsyncEither<int,int>.Other(-1)
            .Then(value => { fired = true; return Either<int,int>.Value(value + 1); })            
            .ValueOr(1);

        Assert.False(fired);
        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task ThenBindAsyncEvaluatesOnValue()
    {                
        var actual = await AsyncEither<int,int>.Value(0)
            .Then(value => AsyncEither<int,int>.Value(value + 1))
            .ValueOr(-1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task ThenBindAsyncDoesNotEvaluateOnOther()
    {                    
        var fired = false;
        var actual = await AsyncEither<int,int>.Other(-1)
            .Then(value => { fired = true; return AsyncEither<int,int>.Value(value + 1); })  
            .ValueOr(1);

        Assert.False(fired);
        Assert.Equal(1, actual);
    }

    //-------------------------------------------------------------------------- Then/Void

    [Fact]
    public async Task ThenVoidEvaluatesOnValue()
    {                
        var fired = false;
        await AsyncEither<int,int>.Value(0)
            .Then(value => { fired = true; })
            .Ignore();

        Assert.True(fired);
    }

    [Fact]
    public async Task ThenVoidDoesNotEvaluateOnOther()
    {                    
        var fired = false;
        await AsyncEither<int,int>.Other(-1)
            .Then(value => { fired = true; })
            .Ignore();

        Assert.False(fired);        
    }

    [Fact]
    public async Task ThenVoidAsyncEvaluatesOnValue()
    {   
        var fired = false;             
        await AsyncEither<int,int>.Value(0)
            .Then(value => { fired = true; return Task.CompletedTask; })
            .Ignore();

        Assert.True(fired);
    }

    [Fact]
    public async Task ThenVoidAsyncDoesNotEvaluateOnOther()
    {                    
        var fired = false;
        await AsyncEither<int,int>.Other(-1)
            .Then(value => { fired = true; return Task.CompletedTask; })
            .Ignore();

        Assert.False(fired);        
    }

    //========================================================================== Compose Right

    //-------------------------------------------------------------------------- Else/Map

    [Fact]
    public async Task ElseMapEvaluatesOnOther()
    {                
        var actual = await AsyncEither<int,int>.Other(0)
            .Else(value => value + 1)
            .ValueOr(-1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task ElseMapDoesNotEvaluateOnValue()
    {                    
        var fired = false;
        await AsyncEither<int,int>.Value(-1)
            .Else(value => { fired = true; return value + 1; })            
            .Ignore();

        Assert.False(fired);        
    }

    [Fact]
    public async Task ElseMapAsyncEvaluatesOnOther()
    {                
        var actual = await AsyncEither<int,int>.Other(0)
            .Else(value => Task.FromResult(value + 1))
            .ValueOr(-1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task ElseMapAsyncDoesNotEvaluateOnValue()
    {                    
        var fired = false;
        var actual = await AsyncEither<int,int>.Value(1)
            .Else(value => { fired = true; return Task.FromResult(value + 1); })  
            .ValueOr(-1);

        Assert.False(fired);
        Assert.Equal(1, actual);
    }    

    //-------------------------------------------------------------------------- Else/Bind

    [Fact]
    public async Task ElseBindEvaluatesOnOther()
    {                
        var actual = await AsyncEither<int,int>.Other(0)
            .Else(value => Either<int,int>.Value(value + 1))
            .ValueOr(-1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task ElseBindDoesNotEvaluateOnValue()
    {                    
        var fired = false;
        await AsyncEither<int,int>.Value(-1)
            .Else(value => { fired = true; return Either<int,int>.Value(value + 1); })            
            .Ignore();

        Assert.False(fired);        
    }

    [Fact]
    public async Task ElseBindAsyncEvaluatesOnOther()
    {                
        var actual = await AsyncEither<int,int>.Other(0)
            .Else(value => AsyncEither<int,int>.Value(value + 1))
            .ValueOr(-1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task ElseBindAsyncDoesNotEvaluateOnValue()
    {                    
        var fired = false;
        var actual = await AsyncEither<int,int>.Value(1)
            .Else(value => { fired = true; return AsyncEither<int,int>.Value(value + 1); })  
            .ValueOr(-1);

        Assert.False(fired);
        Assert.Equal(1, actual);
    }    

    //-------------------------------------------------------------------------- Else/Void

    [Fact]
    public async Task ElseVoidEvaluatesOnOther()
    {                
        var fired = false;
        await AsyncEither<int,int>.Other(0)
            .Else(value => { fired = true; })
            .Ignore();

        Assert.True(fired);
    }

    [Fact]
    public async Task ElseVoidDoesNotEvaluateOnValue()
    {                    
        var fired = false;
        await AsyncEither<int,int>.Value(-1)
            .Else(value => { fired = true; })
            .Ignore();

        Assert.False(fired);        
    }

    [Fact]
    public async Task ElseVoidAsyncEvaluatesOnOther()
    {   
        var fired = false;             
        await AsyncEither<int,int>.Other(0)
            .Else(value => { fired = true; return Task.CompletedTask; })
            .Ignore();

        Assert.True(fired);
    }

    [Fact]
    public async Task ElseVoidAsyncDoesNotEvaluateOnValue()
    {                    
        var fired = false;
        await AsyncEither<int,int>.Value(-1)
            .Else(value => { fired = true; return Task.CompletedTask; })
            .Ignore();

        Assert.False(fired);        
    }

    //========================================================================== Adapter Left

    //-------------------------------------------------------------------------- Filter

    [Fact]
    public async Task FilterReturnsStaticOtherOnFalse()
    {
        var actual = await AsyncEither<int,int>.Value(3)
            .Filter(value => value % 2 == 0, 1)
            .Match(Self, Self);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task FilterReturnsFuncOtherOnFalse()
    {
        var actual = await AsyncEither<int,int>.Value(3)
            .Filter(value => value % 2 == 0, value => 1)
            .Match(Self, Self);

        Assert.Equal(1, actual);
    }

    //========================================================================== Combinator Left

    //-------------------------------------------------------------------------- Or

    [Fact]
    public async Task OrStaticReturnsOriginalOnValue()
    {
        var original = AsyncEither<int,int>.Value(1);
        var alternate = Either<int,int>.Value(2);

        var actual = await original.Or(alternate)
            .ValueOr(-1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task OrStaticReturnsAlternateOnOther()
    {
        var original = AsyncEither<int,int>.Other(1);
        var alternate = Either<int,int>.Value(2);

        var actual = await original.Or(alternate)
            .ValueOr(-1);

        Assert.Equal(2, actual);
    }

    [Fact]
    public async Task OrFuncReturnsOriginalOnValue()
    {
        var original = AsyncEither<int,int>.Value(1);
        var alternate = Either<int,int>.Value(2);

        var actual = await original.Or(() => alternate)
            .ValueOr(-1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task OrFuncReturnsAlternateOnOther()
    {
        var original = AsyncEither<int,int>.Other(1);
        var alternate = Either<int,int>.Value(2);

        var actual = await original.Or(() => alternate)
            .ValueOr(-1);

        Assert.Equal(2, actual);
    }

    [Fact]
    public async Task OrFuncTaskReturnsOriginalOnValue()
    {
        var original = AsyncEither<int,int>.Value(1);
        var alternate = Either<int,int>.Value(2);

        var actual = await original.Or(() => Task.FromResult(alternate))
            .ValueOr(-1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task OrFuncTaskReturnsAlternateOnOther()
    {
        var original = AsyncEither<int,int>.Other(1);
        var alternate = Either<int,int>.Value(2);

        var actual = await original.Or(() => Task.FromResult(alternate))
            .ValueOr(-1);

        Assert.Equal(2, actual);
    }

    //-------------------------------------------------------------------------- And

    [Fact]
    public async Task AndStaticReturnsSelectedOnSuccess()
    {
        var a = AsyncEither<int,int>.Value(1);
        var b = Either<int,int>.Value(2);

        var actual = await a.And(b, (a,b) => a + b)
            .ValueOr(-1);

        Assert.Equal(3, actual);
    }

    [Fact]
    public async Task AndStaticReturnsOtherOnAOther()
    {
        var a = AsyncEither<int,int>.Other(1);
        var b = Either<int,int>.Value(2);

        var actual = await a.And(b, (a,b) => a + b)
            .Match(Self, Self);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task AndStaticReturnsOtherOnBOther()
    {
        var a = AsyncEither<int,int>.Value(1);
        var b = Either<int,int>.Other(2);

        var actual = await a.And(b, (a,b) => a + b)
            .Match(Self, Self);

        Assert.Equal(2, actual);
    }

    [Fact]
    public async Task AndFuncReturnsSelectedOnSuccess()
    {
        var a = AsyncEither<int,int>.Value(1);
        var b = Either<int,int>.Value(2);

        var actual = await a.And(() => b, (a,b) => a + b)
            .ValueOr(-1);

        Assert.Equal(3, actual);
    }

    [Fact]
    public async Task AndFuncStaticReturnsOtherOnAOther()
    {
        var a = AsyncEither<int,int>.Other(1);
        var b = Either<int,int>.Value(2);

        var actual = await a.And(() => b, (a,b) => a + b)
            .Match(Self, Self);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task AndFuncStaticReturnsOtherOnBOther()
    {
        var a = AsyncEither<int,int>.Value(1);
        var b = Either<int,int>.Other(2);

        var actual = await a.And(() => b, (a,b) => a + b)
            .Match(Self, Self);

        Assert.Equal(2, actual);
    }

    [Fact]
    public async Task AndFuncAsyncReturnsSelectedOnSuccess()
    {
        var a = AsyncEither<int,int>.Value(1);
        var b = AsyncEither<int,int>.Value(2);

        var actual = await a.And(() => b, (a,b) => a + b)
            .ValueOr(-1);

        Assert.Equal(3, actual);
    }

    [Fact]
    public async Task AndFuncAsyncStaticReturnsOtherOnAOther()
    {
        var a = AsyncEither<int,int>.Other(1);
        var b = AsyncEither<int,int>.Value(2);

        var actual = await a.And(() => b, (a,b) => a + b)
            .Match(Self, Self);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task AndFuncAsyncStaticReturnsOtherOnBOther()
    {
        var a = AsyncEither<int,int>.Value(1);
        var b = AsyncEither<int,int>.Other(2);

        var actual = await a.And(() => b, (a,b) => a + b)
            .Match(Self, Self);

        Assert.Equal(2, actual);
    }

    //========================================================================== Factory

    [Fact]
    public void LeftFactoryInitializesLeftValue()
    {
        var actual = ((Either<int,string>) Either.Value(1))
            .ValueOr(-1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public void RightFactoryInitializesRightValue()
    {
        ((Either<int,string>) Either.Other("test"))
            .HasOther(out var actual);

        Assert.Equal("test", actual);
    }

    //========================================================================== Test Fixture

    private class AsyncEither<A,B>
    {
        public static Task<Either<A,B>> Value(A value) =>
            Task.FromResult(Either<A,B>.Value(value));

        public static Task<Either<A,B>> Other(B value) =>
            Task.FromResult(Either<A,B>.Other(value));
    }
}
