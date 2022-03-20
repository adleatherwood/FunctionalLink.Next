using Xunit;
// ReSharper disable UnusedParameter.Local
namespace FunctionalLink.Next.Tests;

public class ResultTaskTests
{
    [Fact]
    public async Task AsyncMatchVoidOnSuccess()
    {
        var actual = 1;
        await TaskSuccess(1)
            .Match(
                success => { actual += + 1; },
                failure => { actual = 0; });

        Assert.Equal(2, actual);
    }

    [Fact]
    public async Task AsyncMatchVoidOnFailure()
    {
        var actual = 1;
        await TaskFailure<int>("fail")
            .Match(
                success => { actual += 1; },
                failure => { actual = 0; });

        Assert.Equal(0, actual);
    }

    [Fact]
    public async Task AsyncMatchFuncOnSuccess()
    {
        var actual = await TaskSuccess(1)
            .Match(
                success => success + 1,
                failure => 0);

        Assert.Equal(2, actual);
    }

    [Fact]
    public async Task AsyncMatchFuncOnFailure()
    {
        var actual = await TaskFailure<int>("fail")
            .Match(
                success => success + 1,
                failure => 0);

        Assert.Equal(0, actual);
    }

    [Fact]
    public async Task SyncToAsyncThenBindsSuccessfully()
    {
        var actual = await Result<int>.Success(1)
            .Then(i => TaskSuccess(i + 1))
            .ValueOr(0);

        Assert.Equal(2, actual);
    }

    [Fact]
    public async Task AsyncToAsyncThenBindsSuccessfully()
    {
        var actual = await TaskSuccess(1)
            .Then(i => TaskSuccess(i + 1))
            .ValueOr(0);

        Assert.Equal(2, actual);
    }

    [Fact]
    public async Task AsyncToSyncThenBindsSuccessfully()
    {
        var actual = await TaskSuccess(1)
            .Then(i => Result<int>.Success(i + 1))
            .ValueOr(0);

        Assert.Equal(2, actual);
    }

    [Fact]
    public async Task SyncToAsyncThenMapsSuccessfully()
    {
        var actual = await Result<int>.Success(1)
            .Then(i => Task.FromResult(i + 1))
            .ValueOr(0);

        Assert.Equal(2, actual);
    }

    [Fact]
    public async Task AsyncToAsyncThenMapsSuccessfully()
    {
        var actual = await TaskSuccess(1)
            .Then(i => Task.FromResult(i + 1))
            .ValueOr(0);

        Assert.Equal(2, actual);
    }

    [Fact]
    public async Task AsyncToSyncThenMapsSuccessfully()
    {
        var actual = await TaskSuccess(1)
            .Then(i => i + 1)
            .ValueOr(0);

        Assert.Equal(2, actual);
    }

    [Fact]
    public async Task SyncToAsyncThenVoidsSuccessfully()
    {
        var actual = 0;
        await Result<int>.Success(1)
            .Then(async i => { actual = await Task.FromResult(i + 1); })
            .ValueOr(0);

        Assert.Equal(2, actual);
    }

    [Fact]
    public async Task AsyncToAsyncThenVoidsSuccessfully()
    {
        var actual = 0;
        await TaskSuccess(1)
            .Then(async i => { actual = await Task.FromResult(i + 1); })
            .ValueOr(0);

        Assert.Equal(2, actual);
    }

    [Fact]
    public async Task AsyncToSyncThenVoidsSuccessfully()
    {
        var actual = 0;
        await TaskSuccess(1)
            .Then(i => { actual = i + 1; })
            .ValueOr(0);

        Assert.Equal(2, actual);
    }

    [Fact]
    public async Task SyncToAsyncThenLiftsSuccessfully()
    {
        Result<int> AddOne(int n) => 
            Result<int>.Success(n + 1);

        var actual = await Result<int,string>.Success(1)
            .Then(i => Task.FromResult(AddOne(i)))
            .ValueOr(0);

        Assert.Equal(2, actual);
    }

    [Fact]
    public async Task AsyncToAsyncThenLiftsSuccessfully()
    {
        Result<int> AddOne(int n) => 
            Result<int>.Success(n + 1);

        var actual = await TaskSuccess(1)
            .Then(i => Task.FromResult(AddOne(i)))
            .ValueOr(0);

        Assert.Equal(2, actual);
    }

    [Fact]
    public async Task AsyncToSyncThenLiftsSuccessfully()
    {
        Result<int> AddOne(int n) => 
            Result<int>.Success(n + 1);

        var actual = await TaskSuccess(1)
            .Then(AddOne)
            .ValueOr(0);

        Assert.Equal(2, actual);
    }

    [Fact]
    public async Task AsyncToSyncThenLiftsSuccessfullyForFailure()
    {
        Result<int> AddOne(int n) => 
            Result<int>.Failure("fail");

        var actual = await TaskSuccess(1)
            .Then(AddOne)
            .ValueOr(0);

        Assert.Equal(0, actual);
    }
    
    // valueOr

    [Fact]
    public async Task AsyncToSyncOrsValueSuccessfully()
    {
        var actual = await TaskSuccess(1).Or(2)
            .ValueOr(0);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task AsyncToSyncOrMapsSuccessfully()
    {
        var actual = await TaskSuccess(1).Or(() => 2)
            .ValueOr(0);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task AsyncToSyncOrBindsValueSuccessfully()
    {
        var actual = await TaskSuccess(1).Or(Result<int>.Success(2))
            .ValueOr(0);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task SyncToAsyncOrBindsSuccessfully()
    {
        var actual = await Result<int>.Success(1).Or(() => TaskSuccess(2))
            .ValueOr(0);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task AsyncToAsyncOrBindsSuccessfully()
    {
        var actual = await TaskSuccess(1).Or(() => TaskSuccess(2))
            .ValueOr(0);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task AsyncToSyncOrBindsSuccessfully()
    {
        var actual = await TaskSuccess(1).Or(() => Result<int>.Success(2))
            .ValueOr(0);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task AsyncToSyncAndMapsStaticSuccessfully()
    {
        var actual = await TaskSuccess(1).And(2, (a, b) => a + b)
            .ValueOr(0);

        Assert.Equal(3, actual);
    }

    [Fact]
    public async Task AsyncToSyncAndMapsLazySuccessfully()
    {
        var actual = await TaskSuccess(1).And(() => 2, (a, b) => a + b)
            .ValueOr(0);

        Assert.Equal(3, actual);
    }

    [Fact]
    public async Task AsyncToSyncAndBindsStaticSuccessfully()
    {
        var actual = await TaskSuccess(1).And(Result<int>.Success(2), (a, b) => a + b)
            .ValueOr(0);

        Assert.Equal(3, actual);
    }

    [Fact]
    public async Task AsyncToSyncAndBindsLazySuccessfully()
    {
        var actual = await TaskSuccess(1).And(() => Result<int>.Success(2), (a, b) => a + b)
            .ValueOr(0);

        Assert.Equal(3, actual);
    }

    [Fact]
    public async Task SyncToAsyncAndBindsLazySuccessfully()
    {
        var actual = await Result<int>.Success(1).And(() => TaskSuccess(2), (a, b) => a + b)
            .ValueOr(0);

        Assert.Equal(3, actual);
    }

    [Fact]
    public async Task AsyncToAsyncAndBindsLazySuccessfully()
    {
        var actual = await TaskSuccess(1).And(() => TaskSuccess(2), (a, b) => a + b)
            .ValueOr(0);

        Assert.Equal(3, actual);
    }

    private static Task<Result<T, string>> TaskSuccess<T>(T value) =>
        Task.FromResult(Result<T>.Success(value));

    private static Task<Result<T, string>> TaskFailure<T>(string message) =>
        Task.FromResult(Result<T>.Failure(message));
}