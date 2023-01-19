using Xunit;
using static FunctionalLink.Next.GlobalLink;

namespace FunctionalLink.Next.Tests;

#pragma warning disable 8620
#pragma warning disable 8625

public class OptionTests
{
    [Fact]
    public async Task LeftConstructorMatchesProperly()
    {
        var actual = await AsyncOption<int>.Some(1)
            .Match(
                value => value,
                () => -1);
        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task RightConstructorMatchesProperly()
    {
        var actual = await AsyncOption<int>.None()
            .Match(
                value => -1,
                () => 1);
        Assert.Equal(1, actual);
    }

    [Fact]
    public void ImplicitConversionFromLeftMatchesProperly()
    {
        var value = 1;
        var actual = ((Option<int>) value)
            .Match(
                value => value,
                () => -1);
        Assert.Equal(1, actual);
    }

    [Fact]
    public void ImplicitConversionFromRightMatchesProperly()
    {
        var value = None();
        var actual = ((Option<int>) value)
            .Match(
                value => -1,
                () => 1);
        Assert.Equal(1, actual);
    }

    //========================================================================== Inspect

    [Fact]
    public async Task HasValueIsTrueOnValue()
    {        
        var actual = await AsyncOption<int>.Some(1)
            .HasSome();

        Assert.True(actual);
    }

    [Fact]
    public async Task HasValueIsFalseOnOther()
    {        
        var actual = await AsyncOption<int>.None()
            .HasSome();

        Assert.False(actual);
    }

    [Fact]
    public async Task HasOtherIsTrueOnOther()
    {        
        var actual = await AsyncOption<int>.None()
            .HasNone();

        Assert.True(actual);
    }

    [Fact]
    public async Task HasValueIsFalseOnValue()
    {        
        var actual = await AsyncOption<int>.Some(1)
            .HasNone();

        Assert.False(actual);
    }

    //========================================================================== Decompose 

    //-------------------------------------------------------------------------- Has

    [Fact]
    public void HasValueOutReturnsOnValue()
    {        
        Option<int>.Some(1)
            .HasSome(out var actual)
            .Ignore();

        Assert.Equal(1, actual);
    }

    //-------------------------------------------------------------------------- Match

    [Fact]
    public async Task MatchActionEvaluatesOnValue()
    {        
        var actual = 0;
        await AsyncOption<int>.Some(1)
            .Match(
                value => { actual = value; },
                () => { actual = -1; });

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task MatchActionEvaluatesOnOther()
    {        
        var actual = 0;
        await AsyncOption<int>.None()
            .Match(
                value => { actual = -1; },
                () => { actual = 1; });

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task MatchActionAsyncEvaluatesOnValue()
    {        
        var actual = 0;
        await AsyncOption<int>.Some(1)
            .Match(
                value => { actual = value; ; return Task.CompletedTask; },
                () => { actual = -1; ; return Task.CompletedTask; });

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task MatchActionAsyncEvaluatesOnOther()
    {        
        var actual = 0;
        await AsyncOption<int>.None()
            .Match(
                value => { actual = -1; return Task.CompletedTask; },
                () => { actual = 1; ; return Task.CompletedTask; });

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task MatchFuncEvaluatesOnValue()
    {                
        var actual = await AsyncOption<int>.Some(1)
            .Match(
                value => value,
                () => -1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task MatchFuncEvaluatesOnOther()
    {                
        var actual = await AsyncOption<int>.None()
            .Match(
                value => -1,
                () => 1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task MatchFuncEvaluationsCanBeAsync()
    {                
        var actual = await AsyncOption<int>.Some(1)
            .Match(
                value => value,
                () => -1);
                // value => Task.FromResult(value),
                // other => Task.FromResult(-1));

        Assert.Equal(1, actual);
    }

    //-------------------------------------------------------------------------- ValueOr

    [Fact]
    public async Task ValueOrIsValueOnValue()
    {                
        var actual = await AsyncOption<int>.Some(1)
            .ValueOr(-1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task ValueOrIsOtherOnOther()
    {                
        var actual = await AsyncOption<int>.None()
            .ValueOr(1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task ValueOrAcceptsNullWithoutAmbiguity()
    {   
        var actual = await AsyncOption<string>.None()
            .ValueOr(null);

        Assert.Null(actual);
    }

    //========================================================================== Compose Left

    //-------------------------------------------------------------------------- Then/Map

    [Fact]
    public async Task ThenMapEvaluatesOnValue()
    {                
        var actual = await AsyncOption<int>.Some(0)
            .Then(value => value + 1)
            .ValueOr(-1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task ThenMapDoesNotEvaluateOnOther()
    {                    
        var fired = false;
        var actual = await AsyncOption<int>.None()
            .Then(value => { fired = true; return value + 1; })            
            .ValueOr(1);

        Assert.False(fired);
        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task ThenMapAsyncEvaluatesOnValue()
    {                
        var actual = await AsyncOption<int>.Some(0)
            .Then(value => Task.FromResult(value + 1))
            .ValueOr(-1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task ThenMapAsyncDoesNotEvaluateOnOther()
    {                    
        var fired = false;
        var actual = await AsyncOption<int>.None()
            .Then(value => { fired = true; return Task.FromResult(value + 1); })  
            .ValueOr(1);

        Assert.False(fired);
        Assert.Equal(1, actual);
    }

    //-------------------------------------------------------------------------- Then/Bind

    [Fact]
    public async Task ThenBindEvaluatesOnValue()
    {                
        var actual = await AsyncOption<int>.Some(0)
            .Then(value => Option<int>.Some(value + 1))
            .ValueOr(-1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task ThenBindDoesNotEvaluateOnOther()
    {                    
        var fired = false;
        var actual = await AsyncOption<int>.None()
            .Then(value => { fired = true; return Option<int>.Some(value + 1); })            
            .ValueOr(1);

        Assert.False(fired);
        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task ThenBindAsyncEvaluatesOnValue()
    {                
        var actual = await AsyncOption<int>.Some(0)
            .Then(value => AsyncOption<int>.Some(value + 1))
            .ValueOr(-1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task ThenBindAsyncDoesNotEvaluateOnOther()
    {                    
        var fired = false;
        var actual = await AsyncOption<int>.None()
            .Then(value => { fired = true; return AsyncOption<int>.Some(value + 1); })  
            .ValueOr(1);

        Assert.False(fired);
        Assert.Equal(1, actual);
    }

    //-------------------------------------------------------------------------- Then/Void

    [Fact]
    public async Task ThenVoidEvaluatesOnValue()
    {                
        var fired = false;
        await AsyncOption<int>.Some(0)
            .Then(value => { fired = true; })
            .Ignore();

        Assert.True(fired);
    }

    [Fact]
    public async Task ThenVoidDoesNotEvaluateOnOther()
    {                    
        var fired = false;
        await AsyncOption<int>.None()
            .Then(value => { fired = true; })
            .Ignore();

        Assert.False(fired);        
    }

    [Fact]
    public async Task ThenVoidAsyncEvaluatesOnValue()
    {   
        var fired = false;             
        await AsyncOption<int>.Some(0)
            .Then(value => { fired = true; return Task.CompletedTask; })
            .Ignore();

        Assert.True(fired);
    }

    [Fact]
    public async Task ThenVoidAsyncDoesNotEvaluateOnOther()
    {                    
        var fired = false;
        await AsyncOption<int>.None()
            .Then(value => { fired = true; return Task.CompletedTask; })
            .Ignore();

        Assert.False(fired);        
    }

    //========================================================================== Compose Right

    //-------------------------------------------------------------------------- Else/Map

    [Fact]
    public async Task ElseMapEvaluatesOnOther()
    {                
        var actual = await AsyncOption<int>.None()
            .Else(() => 1)
            .ValueOr(-1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task ElseMapDoesNotEvaluateOnValue()
    {                    
        var fired = false;
        await AsyncOption<int>.Some(-1)
            .Else(() => { fired = true; return 1; })            
            .Ignore();

        Assert.False(fired);        
    }

    [Fact]
    public async Task ElseMapAsyncEvaluatesOnOther()
    {                
        var actual = await AsyncOption<int>.None()
            .Else(() => Task.FromResult(1))
            .ValueOr(-1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task ElseMapAsyncDoesNotEvaluateOnValue()
    {                    
        var fired = false;
        var actual = await AsyncOption<int>.Some(1)
            .Else(() => { fired = true; return Task.FromResult(2); })  
            .ValueOr(-1);

        Assert.False(fired);
        Assert.Equal(1, actual);
    }    

    //-------------------------------------------------------------------------- Else/Bind

    [Fact]
    public async Task ElseBindEvaluatesOnOther()
    {                
        var actual = await AsyncOption<int>.None()
            .Else(() => Option<int>.Some(1))
            .ValueOr(-1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task ElseBindDoesNotEvaluateOnValue()
    {                    
        var fired = false;
        await AsyncOption<int>.Some(-1)
            .Else(() => { fired = true; return Option<int>.Some(1); })            
            .Ignore();

        Assert.False(fired);        
    }

    [Fact]
    public async Task ElseBindAsyncEvaluatesOnOther()
    {                
        var actual = await AsyncOption<int>.None()
            .Else(() => AsyncOption<int>.Some(1))
            .ValueOr(-1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task ElseBindAsyncDoesNotEvaluateOnValue()
    {                    
        var fired = false;
        var actual = await AsyncOption<int>.Some(1)
            .Else(() => { fired = true; return AsyncOption<int>.Some(2); })  
            .ValueOr(-1);

        Assert.False(fired);
        Assert.Equal(1, actual);
    }    

    //-------------------------------------------------------------------------- Else/Void

    [Fact]
    public async Task ElseVoidEvaluatesOnOther()
    {                
        var fired = false;
        await AsyncOption<int>.None()
            .Else(() => { fired = true; });

        Assert.True(fired);
    }

    [Fact]
    public async Task ElseVoidDoesNotEvaluateOnValue()
    {                    
        var fired = false;
        await AsyncOption<int>.Some(-1)
            .Else(() => { fired = true; });

        Assert.False(fired);        
    }

    [Fact]
    public async Task ElseVoidAsyncEvaluatesOnOther()
    {   
        var fired = false;             
        await AsyncOption<int>.None()
            .Else(() => { fired = true; return Task.CompletedTask; });

        Assert.True(fired);
    }

    [Fact]
    public async Task ElseVoidAsyncDoesNotEvaluateOnValue()
    {                    
        var fired = false;
        await AsyncOption<int>.Some(-1)
            .Else(() => { fired = true; return Task.CompletedTask; });

        Assert.False(fired);        
    }

    //========================================================================== Adapter Left

    //-------------------------------------------------------------------------- Filter

    [Fact]
    public async Task FilterReturnsStaticOtherOnFalse()
    {
        var actual = await AsyncOption<int>.Some(3)
            .Filter(value => value % 2 == 0)
            .Match(Self, () => 1);

        Assert.Equal(1, actual);
    }

    //========================================================================== Combinator Left

    //-------------------------------------------------------------------------- Or

    [Fact]
    public async Task OrStaticReturnsOriginalOnValue()
    {
        var original = AsyncOption<int>.Some(1);
        var alternate = Option<int>.Some(2);

        var actual = await original.Or(alternate)
            .ValueOr(-1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task OrStaticReturnsAlternateOnOther()
    {
        var original = AsyncOption<int>.None();
        var alternate = Option<int>.Some(2);

        var actual = await original.Or(alternate)
            .ValueOr(-1);

        Assert.Equal(2, actual);
    }

    [Fact]
    public async Task OrFuncReturnsOriginalOnValue()
    {
        var original = AsyncOption<int>.Some(1);
        var alternate = Option<int>.Some(2);

        var actual = await original.Or(() => alternate)
            .ValueOr(-1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task OrFuncReturnsAlternateOnOther()
    {
        var original = AsyncOption<int>.None();
        var alternate = Option<int>.Some(2);

        var actual = await original.Or(() => alternate)
            .ValueOr(-1);

        Assert.Equal(2, actual);
    }

    [Fact]
    public async Task OrFuncTaskReturnsOriginalOnValue()
    {
        var original = AsyncOption<int>.Some(1);
        var alternate = Option<int>.Some(2);

        var actual = await original.Or(() => Task.FromResult(alternate))
            .ValueOr(-1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task OrFuncTaskReturnsAlternateOnOther()
    {
        var original = AsyncOption<int>.None();
        var alternate = Option<int>.Some(2);

        var actual = await original.Or(() => Task.FromResult(alternate))
            .ValueOr(-1);

        Assert.Equal(2, actual);
    }

    //-------------------------------------------------------------------------- And

    [Fact]
    public async Task AndStaticReturnsSelectedOnSuccess()
    {
        var a = AsyncOption<int>.Some(1);
        var b = Option<int>.Some(2);

        var actual = await a.And(b, (a,b) => a + b)
            .ValueOr(-1);

        Assert.Equal(3, actual);
    }

    [Fact]
    public async Task AndStaticReturnsOtherOnAOther()
    {
        var a = AsyncOption<int>.None();
        var b = Option<int>.Some(2);

        var actual = await a.And(b, (a,b) => a + b)
            .Match(Self, () => 1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task AndStaticReturnsOtherOnBOther()
    {
        var a = AsyncOption<int>.Some(1);
        var b = Option<int>.None();

        var actual = await a.And(b, (a,b) => a + b)
            .Match(Self, () => 2);

        Assert.Equal(2, actual);
    }

    [Fact]
    public async Task AndFuncReturnsSelectedOnSuccess()
    {
        var a = AsyncOption<int>.Some(1);
        var b = Option<int>.Some(2);

        var actual = await a.And(() => b, (a,b) => a + b)
            .ValueOr(-1);

        Assert.Equal(3, actual);
    }

    [Fact]
    public async Task AndFuncStaticReturnsOtherOnAOther()
    {
        var a = AsyncOption<int>.None();
        var b = Option<int>.Some(2);

        var actual = await a.And(() => b, (a,b) => a + b)
            .Match(Self, () => 1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task AndFuncStaticReturnsOtherOnBOther()
    {
        var a = AsyncOption<int>.Some(1);
        var b = Option<int>.None();

        var actual = await a.And(() => b, (a,b) => a + b)
            .Match(Self, () => 2);

        Assert.Equal(2, actual);
    }

    [Fact]
    public async Task AndFuncAsyncReturnsSelectedOnSuccess()
    {
        var a = AsyncOption<int>.Some(1);
        var b = AsyncOption<int>.Some(2);

        var actual = await a.And(() => b, (a,b) => a + b)
            .ValueOr(-1);

        Assert.Equal(3, actual);
    }

    [Fact]
    public async Task AndFuncAsyncStaticReturnsOtherOnAOther()
    {
        var a = AsyncOption<int>.None();
        var b = AsyncOption<int>.Some(2);

        var actual = await a.And(() => b, (a,b) => a + b)
            .Match(Self, () => 1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task AndFuncAsyncStaticReturnsOtherOnBOther()
    {
        var a = AsyncOption<int>.Some(1);
        var b = AsyncOption<int>.None();

        var actual = await a.And(() => b, (a,b) => a + b)
            .Match(Self, () => 2);

        Assert.Equal(2, actual);
    }

    //========================================================================== Factory

    [Fact]
    public void LeftFactoryInitializesLeftValue()
    {
        var actual = Option.Some(1)
            .ValueOr(-1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public void RightFactoryInitializesRightValue()
    {
        var actual = ((Option<int>) Option.None())
            .HasNone();

        Assert.True(actual);
    }

    [Fact]
    public void MaybeFactoryInitializesLeftValue()
    {
        var actual = Option.Maybe(new object())
            .HasSome();

        Assert.True(actual);
    }

    [Fact]
    public void MaybeFactoryInitializesRightValueOnNull()
    {        
        var actual = Option.Maybe<object>(null)
            .HasSome();

        Assert.False(actual);
    }

    //========================================================================== Test Fixture

    private class AsyncOption<A>
    {
        public static Task<Option<A>> Some(A value) =>
            Task.FromResult(Option<A>.Some(value));

        public static Task<Option<A>> None() =>
            Task.FromResult(Option<A>.None());
    }
}
