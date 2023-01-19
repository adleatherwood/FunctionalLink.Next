namespace FunctionalLink.Next;

#pragma warning disable 1591
#pragma warning disable 1998
#pragma warning disable 8604
#pragma warning disable 8625

public class Result<T>
{
    internal readonly int flag;
    internal readonly T success;
    internal readonly Error failure;

    protected Result(int flag, T value, Error failure) =>
        (this.flag, this.success, this.failure) = (flag, value, failure);

    public static Result<T> Success(T value) =>
        new(1, value, default);

    public static Result<T> Failure(string failure) =>
        new(2, default, new Error(failure));

    public static Result<T> Failure(Exception failure) =>
        new(2, default, new Error(failure));

    public static Result<T> Failure(Error failure) =>
        new(2, default, failure);

    public static implicit operator Result<T>(Exception failure) =>
        Failure(failure);

    public static implicit operator Result<T>(Error failure) =>
        Failure(failure);

    //========================================================================== Inspect

    public bool HasSuccess() => 
        flag == 1;
    
    public bool HasFailure() => 
        flag == 2;

    //========================================================================== Decompose 

    //-------------------------------------------------------------------------- Has

    public bool HasSuccess(out T value) 
    {
        value = this.success;
        return flag == 1;
    }

    public bool HasFailure(out Error other) 
    {
        other = this.failure;
        return flag == 2;
    }

    //-------------------------------------------------------------------------- Match

    /// <summary>
    /// Match -> Void
    /// </summary>
    public void Match(Action<T> onSuccess, Action<Error> onFailure) 
    {
        if (flag == 1)
            onSuccess(success);
        else
            onFailure(failure);
    }

    /// <summary>
    /// Match -> Task
    /// </summary>
    public async Task Match(Func<T,Task> onSuccess, Func<Error,Task> onFailure) 
    {
        if (flag == 1)
            await onSuccess(success);
        else
            await onFailure(failure);
    }

    /// <summary>
    /// Match -> U
    /// </summary>
    public U Match<U>(Func<T,U> onSuccess, Func<Error,U> onFailure) =>
        flag == 1 
            ? onSuccess(success) 
            : onFailure(failure);

    //-------------------------------------------------------------------------- ValueOr

    public T ValueOr(T alternate) =>
        Match(
            value => value,
            _ => alternate);

    //========================================================================== Compose Left

    //-------------------------------------------------------------------------- Then/Map

    /// <summary>
    /// Map/Left -> T
    /// </summary>
    public Result<U> Then<U>(Func<T,U> f) =>
        Match(
            value => Result<U>.Success(f(value)), 
            other => Result<U>.Failure(other));   

    /// <summary>
    /// Map/Left -> T Async;
    /// </summary>
    public async Task<Result<U>> Then<U>(Func<T,Task<U>> f) =>
        await Match(
            async value => Result<U>.Success(await f(value)), 
            async other => Result<U>.Failure(other));                

    //-------------------------------------------------------------------------- Then/Bind

    /// <summary>
    /// Bind/Left -> Result
    /// </summary>
    public Result<U> Then<U>(Func<T,Result<U>> f) =>
        Match(
            value => f(value).Match(
                value1 => Result<U>.Success(value1),
                other1 => Result<U>.Failure(other1)), 
            other => Result<U>.Failure(other));

    /// <summary>
    /// Bind/Left -> Result Async
    /// </summary>
    public async Task<Result<U>> Then<U>(Func<T,Task<Result<U>>> f) =>
        await Match(
            async value => (await f(value)).Match(
                value1 => Result<U>.Success(value1),
                other1 => Result<U>.Failure(other1)), 
            async other => Result<U>.Failure(other));

    //-------------------------------------------------------------------------- Then/Void

    /// <summary>
    /// Void/Left -> Result
    /// </summary>
    public Result<T> Then(Action<T> f)
    {
        Match(
            value => { f(value); },
            other => { });
        return this;
    }

    /// <summary>
    /// Void/Left -> Result Async
    /// </summary>
    public async Task<Result<T>> Then(Func<T, Task> f)
    {
        await Match(
            async value => { await f(value); },
            async other => { });
        return this;
    }

    //========================================================================== Compose Right

    //-------------------------------------------------------------------------- Else/Map
    
    /// <summary>
    /// Map/Right -> T
    /// </summary>
    public Result<T> Else(Func<Error,T> f) =>
        Match(
            value => Result<T>.Success(value), 
            other => Result<T>.Success(f(other)));   

    /// <summary>
    /// Map/Right -> T Async
    /// </summary>
    public async Task<Result<T>> Else(Func<Error,Task<T>> f) =>
        await Match(
            async value => Result<T>.Success(value), 
            async other => Result<T>.Success(await f(other)));        

    //-------------------------------------------------------------------------- Else/Bind

    /// <summary>
    /// Bind/Right -> Result
    /// </summary>
    public Result<T> Else(Func<Error, Result<T>> f) =>
        Match(
            _ => this, 
            other => f(other));   

    /// <summary>
    /// Bind/Right -> Result Async
    /// </summary>
    public async Task<Result<T>> Else(Func<Error,Task<Result<T>>> f) =>
        await Match(
            async _ => this, 
            async other => await f(other));        

    //-------------------------------------------------------------------------- Else/Void

    /// <summary>
    /// Void/Right -> Void
    /// </summary>
    public void Else(Action<Error> f) =>
        Match(
            _ => {}, 
            other => f(other));   

    /// <summary>
    /// Void/Right -> Task
    /// </summary>
    public async Task Else(Func<Error,Task> f) =>
        await Match(
            async _ => {}, 
            async other => await f(other));        

    //========================================================================== Adapter Left

    //-------------------------------------------------------------------------- Filter

    public Result<T> Filter(Func<T,bool> predicate, Error failure) => 
        Match(
            success => predicate(success) ? this : Failure(failure),
            failure => this);
        
    public Result<T> Filter(Func<T,bool> predicate, Func<T,Error> failure) => 
        Match(
            success => predicate(success) ? this : Failure(failure(success)),
            failure => this);

    //========================================================================== Combinator Left

    //-------------------------------------------------------------------------- Or

    public Result<T> Or(Result<T> other) => 
        Match(
            success => this,
            failure => other);

    public Result<T> Or(Func<Result<T>> other) => 
        Match(
            success => this,
            failure => other());

    public Task<Result<T>> Or(Func<Task<Result<T>>> other) => 
        Match(
            success => Task.FromResult(this),
            failure => other());

    //-------------------------------------------------------------------------- And

    public Result<U> And<TOther,U>(Result<TOther> other, Func<T,TOther, U> selector) => 
        Match(
            success => other.Match(
                success1 => Result<U>.Success(selector(success, success1)),
                failure1 => Result<U>.Failure(failure1)),
            failure => Result<U>.Failure(failure));

    public Result<U> And<TOther,U>(Func<Result<TOther>> other, Func<T,TOther,U> selector) => 
        Match(
            success => (other()).Match(
                success1 => Result<U>.Success(selector(success, success1)),
                failure1 => Result<U>.Failure(failure1)),
            failure => Result<U>.Failure(failure));

    public async Task<Result<U>> And<TOther,U>(Func<Task<Result<TOther>>> other, Func<T,TOther,U> selector) => 
        await Match(
            async success => (await other()).Match(
                success1 => Result<U>.Success(selector(success, success1)),
                failure1 => Result<U>.Failure(failure1)),
            async failure => Result<U>.Failure(failure));
}

