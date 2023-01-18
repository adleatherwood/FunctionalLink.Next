namespace FunctionalLink.Next;

#pragma warning disable 1591

public static class EitherExtensions
{
    //========================================================================== Inspect

    public static async Task<bool> HasValue<S,F>(this Task<Either<S,F>> result) => 
        (await result).HasValue();
            
    public static async Task<bool> HasOther<S,F>(this Task<Either<S,F>> result) => 
        (await result).HasOther();
        
    //========================================================================== Decompose 

    //-------------------------------------------------------------------------- Match

    public static async Task Match<S,F>(this Task<Either<S,F>> result, Action<S> onValue, Action<F> onOther) =>
        (await result).Match(onValue, onOther);
 
    public static async Task Match<S,F>(this Task<Either<S,F>> result, Func<S,Task> onValue, Func<F,Task> onOther) =>
        await (await result).Match(onValue, onOther);

    public static async Task<U> Match<S,F,U>(this Task<Either<S,F>> result, Func<S,U> onValue, Func<F,U> onOther) =>
        (await result).Match(onValue, onOther);

    //-------------------------------------------------------------------------- ValueOr

    public static async Task<S> ValueOr<S,F>(this Task<Either<S,F>> result, S alternate) =>
        (await result).ValueOr(alternate);

    public static async Task<S> ValueOr<S,F>(this Task<Either<S,F>> result, Func<S> alternate) =>
        (await result).ValueOr(alternate);

    public static async Task<S> ValueOr<S,F>(this Task<Either<S,F>> result, Func<Task<S>> alternate) =>
        await (await result).ValueOr(alternate);

    //========================================================================== Compose Value

    //-------------------------------------------------------------------------- Then/Map

    /// <summary>
    /// Async Map/Value -> T 
    /// </summary>   
    public static async Task<Either<U,F>> Then<S,F,U>(this Task<Either<S,F>> result, Func<S,U> f) =>
        (await result).Then(f);

    /// <summary>
    /// Async Map/Value -> T Async
    /// </summary>   
    public static async Task<Either<U,F>> Then<S,F,U>(this Task<Either<S,F>> result, Func<S,Task<U>> f) =>
        await (await result).Then(f);

    //-------------------------------------------------------------------------- Then/Bind

    /// <summary>
    /// Async Bind/Value -> T 
    /// </summary>   
    public static async Task<Either<U,F>> Then<S,F,U>(this Task<Either<S,F>> result, Func<S,Either<U,F>> f) =>
        (await result).Then(f);

    /// <summary>
    /// Async Bind/Value -> T Async
    /// </summary>
    public static async Task<Either<U,F>> Then<S,F,U>(this Task<Either<S,F>> result, Func<S,Task<Either<U,F>>> f) =>
        await (await result).Then(f);

    //-------------------------------------------------------------------------- Then/Void

    /// <summary>
    /// Async Void/Value -> T 
    /// </summary>   
    public static async Task<Either<S,F>> Then<S,F>(this Task<Either<S,F>> result, Action<S> f) =>
        (await result).Then(f);

    /// <summary>
    /// Async Void/Value -> T Async
    /// </summary>
    public static async Task<Either<S,F>> Then<S,F>(this Task<Either<S,F>> result, Func<S, Task> f) =>
        await (await result).Then(f);

    //========================================================================== Compose Other

    //-------------------------------------------------------------------------- Else/Map

    /// <summary>
    /// Async Map/Other -> T 
    /// </summary>   
    public static async Task<Either<S,F>> Else<S,F>(this Task<Either<S,F>> result, Func<F,S> f) =>
        (await result).Else(f);

    /// <summary>
    /// Async Map/Other -> T Async
    /// </summary>
    public static async Task<Either<S,F>> Else<S,F>(this Task<Either<S,F>> result, Func<F,Task<S>> f) =>
        await (await result).Else(f);

    //-------------------------------------------------------------------------- Else/Bind

    /// <summary>
    /// Async Bind/Other -> T 
    /// </summary>   
    public static async Task<Either<S,F>> Else<S,F>(this Task<Either<S,F>> result, Func<F, Either<S,F>> f) =>
        (await result).Else(f);

    /// <summary>
    /// Async Bind/Other -> T Async
    /// </summary>
    public static async Task<Either<S,F>> Else<S,F>(this Task<Either<S,F>> result, Func<F,Task<Either<S,F>>> f) =>
        await (await result).Else(f);

    //-------------------------------------------------------------------------- Else/Void

    /// <summary>
    /// Async Void/Other -> T 
    /// </summary>   
    public static async Task<Either<S,F>> Else<S,F>(this Task<Either<S,F>> result, Action<F> f) =>
        (await result).Else(f);

    /// <summary>
    /// Async Void/Other -> T Async
    /// </summary>
    public static async Task<Either<S,F>> Else<S,F>(this Task<Either<S,F>> result, Func<F,Task> f) =>
        await (await result).Else(f);

    //========================================================================== Adapter Value

    //-------------------------------------------------------------------------- Filter

    public static async Task<Either<S,F>> Filter<S,F>(this Task<Either<S,F>> result, Func<S,bool> predicate, F failure) => 
        (await result).Filter(predicate, failure);

    public static async Task<Either<S,F>> Filter<S,F>(this Task<Either<S,F>> result, Func<S,bool> predicate, Func<S,F> failure) => 
        (await result).Filter(predicate, failure);

    //========================================================================== Combinator Value

    //-------------------------------------------------------------------------- Or

    public static async Task<Either<S,F>> Or<S,F>(this Task<Either<S,F>> result, Either<S,F> other) => 
        (await result).Or(other);

    public static async Task<Either<S,F>> Or<S,F>(this Task<Either<S,F>> result, Func<Either<S,F>> other) => 
        (await result).Or(other);

    public static async Task<Either<S,F>> Or<S,F>(this Task<Either<S,F>> result, Func<Task<Either<S,F>>> other) => 
        await (await result).Or(other);

    //-------------------------------------------------------------------------- And

    public static async Task<Either<U,F>> And<S,F,TOther,U>(this Task<Either<S,F>> result, Either<TOther,F> other, Func<S,TOther, U> selector) => 
        (await result).And(other, selector);

    public static async Task<Either<U,F>> And<S,F,TOther,U>(this Task<Either<S,F>> result, Func<Either<TOther,F>> other, Func<S,TOther,U> selector) => 
        (await result).And(other, selector);

    public static async Task<Either<U,F>> And<S,F,TOther,U>(this Task<Either<S,F>> result, Func<Task<Either<TOther,F>>> other, Func<S,TOther,U> selector) => 
        await (await result).And(other, selector);
}
