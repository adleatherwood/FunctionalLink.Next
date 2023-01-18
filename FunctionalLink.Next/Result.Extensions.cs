namespace FunctionalLink.Next;

#pragma warning disable 1591

public static class ResultExtensions
{
    //========================================================================== Inspect

    public static async Task<bool> HasSuccess<T>(this Task<Result<T>> result) => 
        (await result).HasSuccess();
            
    public static async Task<bool> HasFailure<T>(this Task<Result<T>> result) => 
        (await result).HasFailure();
        
    //========================================================================== Decompose 

    //-------------------------------------------------------------------------- Match

    public static async Task Match<T>(this Task<Result<T>> result, Action<T> onSuccess, Action<Error> onFailure) =>
        (await result).Match(onSuccess, onFailure);
 
    public static async Task Match<T>(this Task<Result<T>> result, Func<T,Task> onSuccess, Func<Error,Task> onFailure) =>
        await (await result).Match(onSuccess, onFailure);

    public static async Task<U> Match<T,U>(this Task<Result<T>> result, Func<T,U> onSuccess, Func<Error,U> onFailure) =>
        (await result).Match(onSuccess, onFailure);

    //-------------------------------------------------------------------------- ValueOr

    public static async Task<T> ValueOr<T>(this Task<Result<T>> result, T alternate) =>
        (await result).ValueOr(alternate);

    public static async Task<T> ValueOr<T>(this Task<Result<T>> result, Func<T> alternate) =>
        (await result).ValueOr(alternate);

    public static async Task<T> ValueOr<T>(this Task<Result<T>> result, Func<Task<T>> alternate) =>
        await (await result).ValueOr(alternate);

    //========================================================================== Compose Left

    //-------------------------------------------------------------------------- Then/Map
    
    public static async Task<Result<U>> Then<T,U>(this Task<Result<T>> result, Func<T,U> f) =>
        (await result).Then(f);

    public static async Task<Result<U>> Then<T,U>(this Task<Result<T>> result, Func<T,Task<U>> f) =>
        await (await result).Then(f);

    //-------------------------------------------------------------------------- Then/Bind

    public static async Task<Result<U>> Then<T,U>(this Task<Result<T>> result, Func<T,Result<U>> f) =>
        (await result).Then(f);

    public static async Task<Result<U>> Then<T,U>(this Task<Result<T>> result, Func<T,Task<Result<U>>> f) =>
        await (await result).Then(f);

    //-------------------------------------------------------------------------- Then/Void

    public static async Task<Result<T>> Then<T>(this Task<Result<T>> result, Action<T> f) =>
        (await result).Then(f);

    public static async Task<Result<T>> Then<T>(this Task<Result<T>> result, Func<T, Task> f) =>
        await (await result).Then(f);

    //========================================================================== Compose Right

    //-------------------------------------------------------------------------- Else/Map

    public static async Task<Result<T>> Else<T>(this Task<Result<T>> result, Func<Error,T> f) =>
        (await result).Else(f);

    public static async Task<Result<T>> Else<T>(this Task<Result<T>> result, Func<Error,Task<T>> f) =>
        await (await result).Else(f);

    //-------------------------------------------------------------------------- Else/Bind

    public static async Task<Result<T>> Else<T>(this Task<Result<T>> result, Func<Error, Result<T>> f) =>
        (await result).Else(f);

    public static async Task<Result<T>> Else<T>(this Task<Result<T>> result, Func<Error,Task<Result<T>>> f) =>
        await (await result).Else(f);

    //-------------------------------------------------------------------------- Else/Void

    public static async Task Else<T>(this Task<Result<T>> result, Action<Error> f) =>
        (await result).Else(f);

    public static async Task Else<T>(this Task<Result<T>> result, Func<Error,Task> f) =>
        await (await result).Else(f);

    //========================================================================== Adapter Left

    //-------------------------------------------------------------------------- Filter

    public static async Task<Result<T>> Filter<T>(this Task<Result<T>> result, Func<T,bool> predicate, Error failure) => 
        (await result).Filter(predicate, failure);

    public static async Task<Result<T>> Filter<T>(this Task<Result<T>> result, Func<T,bool> predicate, Func<T,Error> failure) => 
        (await result).Filter(predicate, failure);

    //========================================================================== Combinator Left

    //-------------------------------------------------------------------------- Or

    public static async Task<Result<T>> Or<T>(this Task<Result<T>> result, Result<T> other) => 
        (await result).Or(other);

    public static async Task<Result<T>> Or<T>(this Task<Result<T>> result, Func<Result<T>> other) => 
        (await result).Or(other);

    public static async Task<Result<T>> Or<T>(this Task<Result<T>> result, Func<Task<Result<T>>> other) => 
        await (await result).Or(other);

    //-------------------------------------------------------------------------- And

    public static async Task<Result<U>> And<T,TOther,U>(this Task<Result<T>> result, Result<TOther> other, Func<T,TOther,U> selector) => 
        (await result).And(other, selector);

    public static async Task<Result<U>> And<T,TOther,U>(this Task<Result<T>> result, Func<Result<TOther>> other, Func<T,TOther,U> selector) => 
        (await result).And(other, selector);

    public static async Task<Result<U>> And<T,TOther,U>(this Task<Result<T>> result, Func<Task<Result<TOther>>> other, Func<T,TOther,U> selector) => 
        await (await result).And(other, selector);
}
