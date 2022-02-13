using Xunit;
using static FunctionalLink.Next.GlobalLink;
// ReSharper disable UnusedParameter.Local

namespace FunctionalLink.Next.Tests;

public class OptionTests
{
    [Fact]
    public void ExampleUsageA()
    {
        // using static FunctionalLink.Next.GlobalLink;
        Option<int> Divide(int a, int b) =>
            b == 0 ? None() : Some(a / b);
        
        var a = Divide(100, 10);
        var b = Divide(100, 5);

        var c = a.And(b, Tuple.Create)
            .Then(tuple => tuple.Item1 + tuple.Item2)
            .Then(total => Divide(total, 10))
            .Then(Console.Write)
            .ValueOr(0);
        
        Assert.Equal(3, c);
    }

    [Fact]
    public void ExampleUsageB()
    {
        // using static FunctionalLink.Next.GlobalLink;
        Option<int> Parse(string value) =>
            int.TryParse(value, out var result) 
                ? Some(result) 
                : None();
        
        Option<string> ReadFile(string filename) =>
            File.Exists(filename)
                ? File.ReadAllText(filename).Maybe()
                : None();

        Option<int> FromEnv(string name) =>
            Environment.GetEnvironmentVariable(name).Maybe()
                .Then(Parse);
        
        Option<int> FromFile(string filename) =>
            ReadFile(filename)
                .Then(Parse);

        var actual = FromEnv("MY_VALUE")
            .Or(() => FromFile("my_value.txt"))
            .ValueOr(1);

        Assert.Equal(1, actual);
    }

    [Fact]
    public void ExampleC()
    {
        Option<int> FindHighScore(int userId) =>
            Some(100);

        void ShowUser(string message) =>
            Console.WriteLine(message);

        var highScore = FindHighScore(1)
            .Then(hs => $"Your Personal Best: {hs}")
            .Else("No scores on record yet")
            .Then(ShowUser)
            .ValueOr("");
        
        Assert.Equal("Your Personal Best: 100", highScore);
    }

    [Fact]
    public void ExampleD()
    {
        Option<(int UserId, string Username)> FindUser(int userId) =>
            Some((userId, "Leroy Jenkins"));
        
        Option<int> FindHighScore(int userId) =>
            Some(100);
        
        var user = FindUser(userId: 1);
        var highScore = FindHighScore(userId: 1);
        user
            .And(highScore, (u, hs) => (u.Username, HighScore: hs))
            .Then(t => $"{t.Username}'s Personal Best: {t.HighScore}")
            .Then(Console.WriteLine);    
    }
    
    [Fact]
    public void InternalConstructorSetsSomeStateCorrectly()
    {
        var found = Option<int>.Some(1)
            .HasValue(out var actual);
        
        Assert.True(found);
        Assert.Equal(1, actual);
    }
    
    [Fact]
    public void InternalConstructorSetsNoneStateCorrectly()
    {
        var found = Option<int>.None()
            .HasValue(out var actual);
        
        Assert.False(found); 
        Assert.Equal(0, actual);
    }

    [Fact]
    public void ImplicitConversionSetsNoneStateCorrectly()
    {
        var found = ((Option<int>) None.Value)
            .HasValue(out var actual);
        
        Assert.False(found); 
        Assert.Equal(0, actual);
    }

    [Fact]
    public void HasValueReturnsValueAndTrueForSomeState()
    {
        var sut = Option.Some(1);
        var found = sut.HasValue(out var actual);
        
        Assert.True(found);
        Assert.Equal(1, actual);
    }

    [Fact]
    public void HasValueReturnsDefaultAndFalseForNoneState()
    {
        var sut = (Option<int>) Option.None();
        var found = sut.HasValue(out var actual);
        
        Assert.False(found);
        Assert.Equal(0, actual);
    }

    [Fact]
    public void MatchExecutesOnSomeActionForSomeState()
    {
        var sut = Option.Some(1);
        var actual = 0;
        
        sut.Match(
            _ => { actual++; },
            _ => { actual--; });
        
        Assert.Equal(1, actual);
    }
    
    [Fact]
    public void MatchExecutesOnNoneActionForNoneState()
    {
        var sut = (Option<int>) Option.None();
        var actual = 0;
        
        sut.Match(
            _ => { actual++; },
            _ => { actual--; });
        
        Assert.Equal(-1, actual);
    }
    
    [Fact]
    public void MatchExecutesOnSomeFuncForSomeState()
    {
        var sut = Option.Some(1);
        var actual = sut.Match(
            some => some + 1,
            _ => -1);
        
        Assert.Equal(2, actual);
    }
    
    [Fact]
    public void MatchExecutesOnNoneFuncForNoneState()
    {
        var sut = (Option<int>) Option.None();
        var actual = sut.Match(
            some => some + 1,
            _ => -1);
        
        Assert.Equal(-1, actual);
    }

    [Fact]
    public void ThenBindExecutesFuncForSomeState()
    {
        var sut = Option.Some(1);
        var found = sut
            .Then(i => Option.Some(i + 1))
            .HasValue(out var actual);
        
        Assert.True(found);
        Assert.Equal(2, actual);
    }
    
    [Fact]
    public void ThenBindDoesNotExecuteFuncForNoneState()
    {
        var sut = (Option<int>) Option.None();
        var actual = sut
            .Then(i => Option.Some(i + 1))
            .ValueOr(0);
        
        Assert.Equal(0, actual);
    }
    
    [Fact]
    public void ThenMapExecutesFuncForSomeState()
    {
        var sut = Option.Some(1);
        var found = sut
            .Then(i => i + 1)
            .HasValue(out var actual);
        
        Assert.True(found);
        Assert.Equal(2, actual);
    }

    [Fact]
    public void ElseStaticSubstitutesGivenValueIfNone()
    {
        Option<int> option = None();
        var actual = option
            .Else(123)
            .ValueOr(0);
        
        Assert.Equal(123, actual);
    }
    
    [Fact]
    public void ElseDynamicSubstitutesGivenValueIfNone()
    {
        Option<int> option = None();
        var actual = option
            .Else(() => 123)
            .ValueOr(0);
        
        Assert.Equal(123, actual);
    }
    
    [Fact]
    public void ThenMapDoesNotExecuteFuncForNoneState()
    {
        var sut = (Option<int>) Option.None();
        var actual = sut
            .Then(i => i + 1)
            .ValueOr(0);
        
        Assert.Equal(0, actual);
    }
    
    [Fact]
    public void ThenActionExecutesActionForSomeState()
    {
        var sut = Option.Some(1);
        var actual = 0;
        
        sut
            .Then(i => { actual = i; })
            .Ignore();
        
        Assert.Equal(1, actual);
    }
    
    [Fact]
    public void ThenActionDoesNotExecuteActionForNoneState()
    {
        var sut = (Option<int>) Option.None();
        var actual = 0;
        
        sut
            .Then(i => { actual = i; })
            .Ignore();
        
        Assert.Equal(0, actual);
    }

    [Fact]
    public void ValueOrReturnsTheValueForSomeState()
    {
        var actual = Option.Some(1)
            .ValueOr(0);
        
        Assert.Equal(1, actual);
    }
    
    [Fact]
    public void ValueOrReturnsTheAlternateForNoneState()
    {
        var actual = ((Option<int>)Option.None())
            .ValueOr(0);
        
        Assert.Equal(0, actual);
    }

    [Fact]
    public void OrStaticReturnsAOverBForSomeStateOfA()
    {
        var a = Option.Some(1);
        var b = (Option<int>) Option.None();

        var actual = a.Or(b)
            .ValueOr(0);
        
        Assert.Equal(1, actual);
    }
    
    [Fact]
    public void OrStaticReturnsBOverAForNoneStateOfA()
    {
        var a = (Option<int>) Option.None();
        var b = Option.Some(1);

        var actual = a.Or(b)
            .ValueOr(0);
        
        Assert.Equal(1, actual);
    }
    
    [Fact]
    public void OrDynamicReturnsAOverBForSomeStateOfA()
    {
        var a = Option.Some(1);
        var b = (Option<int>) Option.None();

        var actual = a.Or(() => b)
            .ValueOr(0);
        
        Assert.Equal(1, actual);
    }
    
    [Fact]
    public void OrDynamicReturnsBOverAForNoneStateOfA()
    {
        var a = (Option<int>) Option.None();
        var b = Option.Some(1);

        var actual = a.Or(() => b)
            .ValueOr(0);
        
        Assert.Equal(1, actual);
    }

    [Fact]
    public void AndStaticReturnsValueForSomeOfAAndB()
    {
        var a = Option.Some(1);
        var b = Option.Some(2);

        var actual = a.And(b, (aa, bb) => aa + bb)
            .ValueOr(0);
        
        Assert.Equal(3, actual);
    }
    
    [Fact]
    public void AndStaticReturnsNoneFormNoneOfA()
    {
        var a = (Option<int>) Option.None();
        var b = Option.Some(2);

        var actual = a.And(b, (aa, bb) => aa + bb)
            .ValueOr(0);
        
        Assert.Equal(0, actual);
    }
    
    [Fact]
    public void AndStaticReturnsNoneFormNoneOfB()
    {
        var a = Option.Some(1);
        var b = (Option<int>) Option.None();

        var actual = a.And(b, (aa, bb) => aa + bb)
            .ValueOr(0);
        
        Assert.Equal(0, actual);
    }
    
    [Fact]
    public void AndDynamicReturnsValueForSomeOfAAndB()
    {
        var a = Option.Some(1);
        var b = Option.Some(2);

        var actual = a.And(() => b, (aa, bb) => aa + bb)
            .ValueOr(0);
        
        Assert.Equal(3, actual);
    }
    
    [Fact]
    public void AndDynamicReturnsNoneFormNoneOfA()
    {
        var a = (Option<int>) Option.None();
        var b = Option.Some(2);

        var actual = a.And(() => b, (aa, bb) => aa + bb)
            .ValueOr(0);
        
        Assert.Equal(0, actual);
    }
    
    [Fact]
    public void AndDynamicReturnsNoneFormNoneOfB()
    {
        var a = Option.Some(1);
        var b = (Option<int>) Option.None();

        var actual = a.And(() => b, (aa, bb) => aa + bb)
            .ValueOr(0);
        
        Assert.Equal(0, actual);
    }
}