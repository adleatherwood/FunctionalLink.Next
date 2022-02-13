using Xunit;
using static FunctionalLink.Next.GlobalLink;
namespace FunctionalLink.Next.Tests;

public class ResultTests
{
    class User
    {
        public string Username;
        public string Email;
        public string Phone;
    }
    
    [Fact]
    public void ExampleA()
    {
        Result<User> ValidateUsername(User user)
        {
            if (user.Username == null)
                return Failure("Username cannot be null");
            
            if (string.IsNullOrWhiteSpace(user.Username))
                return Failure("Username cannot be empty");
            
            return Success(user);
        }

        Result<User> ValidateEmail(User user)
        {
            if (!user.Email.Contains("@"))
                return Failure("Invalid email");

            return Success(user);
        }

        Result<User> ValidatePhone(User user) =>
            string.IsNullOrWhiteSpace(user.Phone)
                ? Failure("Invalid phone number")
                : Success(user);
        
        Result<User> ValidateUser(User user) =>
            ValidateUsername(user)
                .Then(ValidateEmail)
                .Then(ValidatePhone);
        
        var user = new User() {Username = "adleatherwood", Email = "adleatherwood@gmail.com", Phone = "123-456-7890"};
        var actual = ValidateUser(user)
            .Match(
                _ => true,
                _ => false);

        Assert.True(actual);
    }
    
    [Fact]
    public void SuccessConstructorWithValueWorksProperly()
    {
        var actual = Result<int>.Success(1)
            .ValueOr(0);
        
        Assert.Equal(1, actual);
    }

    [Fact]
    public void FailureConstructorWorksProperly()
    {
        var result = Result<int>.Failure("test");
        
        var found = result.HasFailure(out var actual);
        
        Assert.True(found);
        Assert.Equal("test", actual);
    }

    [Fact]
    public void ImplicitConversionFromSuccessWorksForResult1()
    {
        var success = new Success<int>(1);
        var result = (Result<int>) success;
        var actual = result.ValueOr(0);

        Assert.Equal(1, actual);
    }
    
    [Fact]
    public void ImplicitConversionFromSuccessWorksForResult2()
    {
        var success = new Success<int>(1);
        var result = (Result<int,Exception>) success;
        var actual = result.ValueOr(0);

        Assert.Equal(1, actual);
    }
    
    [Fact]
    public void ImplicitConversionFromFailureWorksForResult1()
    {
        var success = new Failure<string>("test");
        var result = (Result<int>) success;
        var found = result.HasFailure(out var actual);

        Assert.True(found);
        Assert.Equal("test", actual);
    }
    
    [Fact]
    public void ImplicitConversionFromFailureWorksForResult2()
    {
        var success = new Failure<Exception>(new Exception("test"));
        var result = (Result<int,Exception>) success;
        var found = result.HasFailure(out var actual);

        Assert.True(found);
        Assert.Equal("test", actual.Message);
    }

    [Fact]
    public void ImplicitConversionFromResult2ToResult1Works()
    {
        var result2 = Result<int, Exception>.Success(1);
        var result1 = (Result<int>) result2;
        var actual = result1.ValueOr(0);
        
        Assert.Equal(1, actual);
    }

    [Fact]
    public void MatchWithActionExecutesSuccessPath()
    {
        var actual = "";
        Result<int>.Success(1)
            .Match(
                success => { actual = success.ToString(); },
                _ => { });
        
        Assert.Equal("1", actual);
    }
    
    [Fact]
    public void MatchWithActionExecutesFailurePath()
    {
        var actual = "";
        Result<int>.Failure("pass")
            .Match(
                _ => { actual = "fail"; },
                failure => { actual = failure; });
        
        Assert.Equal("pass", actual);
    }
    
    [Fact]
    public void MatchWithFuncExecutesSuccessPath()
    {
        var actual = Result<int>.Success(1)
            .Match(
                success => success.ToString(),
                _ => "nope");
        
        Assert.Equal("1", actual);
    }
    
    [Fact]
    public void MatchWithFuncExecutesFailurePath()
    {
        var actual = Result<int>.Failure("pass")
            .Match(
                _ => "nope",
                failure => failure);
        
        Assert.Equal("pass", actual);
    }

    [Fact]
    public void ThenBindExecutesOnSuccess()
    {
        var actual = Result<int>.Success(1)
            .Then(i => Result<int>.Success(i + 1))
            .ValueOr(0);
        
        Assert.Equal(2, actual);
    }
    
    [Fact]
    public void ThenBindWorksWithResult1()
    {
        /* NOTE: this override is what prevents a Result<Result<int>,string> type from being returned instead
         *       of Result<int, string>.
         */
        Result<int> Add(int a, int b) => Result.Success(a + b); 
        var actual = Result<int>.Success(1)
            .Then(i => Add(i, 1))
            .ValueOr(0);
        
        Assert.Equal(2, actual);
    }
    
    [Fact]
    public void ThenMapExecutesOnSuccess()
    {
        var actual = Result<int>.Success(1)
            .Then(i => i + 1)
            .ValueOr(0);
        
        Assert.Equal(2, actual);
    }
    
    [Fact]
    public void ThenActionExecutesOnSuccess()
    {
        var actual = 0;
        Result<int>.Success(1)
            .Then(i => { actual = i + 1; })
            .ValueOr(0);
        
        Assert.Equal(2, actual);
    }

    [Fact]
    public void ElseStaticExecutesOnFailure()
    {
        var actual = Result<int>.Failure("fail")
            .Else(1)
            .ValueOr(0);
        
        Assert.Equal(1, actual);
    }
    
    [Fact]
    public void ElseDynamicExecutesOnFailure()
    {
        var actual = Result<int>.Failure("fail")
            .Else(() => 1)
            .ValueOr(0);
        
        Assert.Equal(1, actual);
    }
    
    [Fact]
    public void OrStaticReturnsAOverBForSuccessStateOfA()
    {
        var a = Result<int>.Success(1);
        var b = Result<int>.Failure("fail");

        var actual = a.Or(b)
            .ValueOr(0);
        
        Assert.Equal(1, actual);
    }
    
    [Fact]
    public void OrStaticReturnsBOverAForFailureStateOfA()
    {
        var a = Result<int>.Failure("fail");
        var b = Result<int>.Success(1);

        var actual = a.Or(b)
            .ValueOr(0);
        
        Assert.Equal(1, actual);
    }
    
    [Fact]
    public void OrDynamicReturnsAOverBForSuccessStateOfA()
    {
        var a = Result<int>.Success(1);
        var b = Result<int>.Failure("fail");

        var actual = a.Or(() => b)
            .ValueOr(0);
        
        Assert.Equal(1, actual);
    }
    
    [Fact]
    public void OrDynamicReturnsBOverAForFailureStateOfA()
    {
        var a = Result<int>.Failure("fail");
        var b = Result<int>.Success(1);

        var actual = a.Or(() => b)
            .ValueOr(0);
        
        Assert.Equal(1, actual);
    }
    
    [Fact]
    public void AndStaticReturnsValueForSuccessOfAAndB()
    {
        var a = Result<int>.Success(1);
        var b = Result<string>.Success("test");

        var actual = a.And(b, (aa, bb) => $"{aa}-{bb}")
            .ValueOr("fail");
        
        Assert.Equal("1-test", actual);
    }
    
    [Fact]
    public void AndStaticReturnsFailureFormFailureOfA()
    {
        var a = Result<int>.Failure("fail");
        var b = Result<string>.Success("test");

        var result = a.And(b, (aa, bb) => $"{aa}-{bb}");
        var found = result.HasFailure(out var actual);
        
        Assert.True(found);
        Assert.Equal("fail", actual);
    }
    
    [Fact]
    public void AndStaticReturnsFailureFormFailureOfB()
    {
        var a = Result<int>.Success(1);
        var b = Result<string>.Failure("fail");

        var result = a.And(b, (aa, bb) => $"{aa}-{bb}");
        var found = result.HasFailure(out var actual);

        Assert.True(found);
        Assert.Equal("fail", actual);
    }
    
    //
    
    [Fact]
    public void AndDynamicReturnsValueForSuccessOfAAndB()
    {
        var a = Result<int>.Success(1);
        var b = Result<string>.Success("test");

        var actual = a.And(() => b, (aa, bb) => $"{aa}-{bb}")
            .ValueOr("fail");
        
        Assert.Equal("1-test", actual);
    }
    
    [Fact]
    public void AndDynamicReturnsFailureFormFailureOfA()
    {
        var a = Result<int>.Failure("fail");
        var b = Result<string>.Success("test");

        var result = a.And(() => b, (aa, bb) => $"{aa}-{bb}");
        var found = result.HasFailure(out var actual);

        
        Assert.True(found);
        Assert.Equal("fail", actual);
    }
    
    [Fact]
    public void AndDynamicReturnsFailureFormFailureOfB()
    {
        var a = Result<int>.Success(1);
        var b = Result<string>.Failure("fail");

        var result = a.And(() => b, (aa, bb) => $"{aa}-{bb}");
        var found = result.HasFailure(out var actual);

        Assert.True(found);
        Assert.Equal("fail", actual);
    }
}