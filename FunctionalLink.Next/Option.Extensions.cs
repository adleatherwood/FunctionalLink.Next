namespace FunctionalLink.Next;

#pragma warning disable 1591

public static class OptionExtensions
{
    //========================================================================== Convert

    //========================================================================== Inspect

    public static async Task<bool> HasSome<T>(this Task<Option<T>> option) => 
        (await option).HasSome();
            
    public static async Task<bool> HasNone<T>(this Task<Option<T>> option) => 
        (await option).HasNone();
        
    //========================================================================== Decompose 

    //-------------------------------------------------------------------------- Match

    public static async Task Match<T>(this Task<Option<T>> option, Action<T> onSome, Action onNone) =>
        (await option).Match(onSome, onNone);
 
    public static async Task Match<T>(this Task<Option<T>> option, Func<T,Task> onSome, Func<Task> onNone) =>
        await (await option).Match(onSome, onNone);

    public static async Task<U> Match<T,U>(this Task<Option<T>> option, Func<T,U> onSome, Func<U> onNone) =>
        (await option).Match(onSome, onNone);

    //-------------------------------------------------------------------------- ValueOr

    public static async Task<T> ValueOr<T>(this Task<Option<T>> option, T alternate) =>
        (await option).ValueOr(alternate);

    //========================================================================== Compose Left

    //-------------------------------------------------------------------------- Then/Map
    
    public static async Task<Option<U>> Then<T,U>(this Task<Option<T>> option, Func<T,U> f) =>
        (await option).Then(f);

    public static async Task<Option<U>> Then<T,U>(this Task<Option<T>> option, Func<T,Task<U>> f) =>
        await (await option).Then(f);

    //-------------------------------------------------------------------------- Then/Bind

    public static async Task<Option<U>> Then<T,U>(this Task<Option<T>> option, Func<T,Option<U>> f) =>
        (await option).Then(f);

    public static async Task<Option<U>> Then<T,U>(this Task<Option<T>> option, Func<T,Task<Option<U>>> f) =>
        await (await option).Then(f);

    //-------------------------------------------------------------------------- Then/Void

    public static async Task<Option<T>> Then<T>(this Task<Option<T>> option, Action<T> f) =>
        (await option).Then(f);

    public static async Task<Option<T>> Then<T>(this Task<Option<T>> option, Func<T, Task> f) =>
        await (await option).Then(f);

    //========================================================================== Compose Right

    //-------------------------------------------------------------------------- Else/Map

    public static async Task<Option<T>> Else<T>(this Task<Option<T>> option, Func<T> f) =>
        (await option).Else(f);

    public static async Task<Option<T>> Else<T>(this Task<Option<T>> option, Func<Task<T>> f) =>
        await (await option).Else(f);

    //-------------------------------------------------------------------------- Else/Bind

    public static async Task<Option<T>> Else<T>(this Task<Option<T>> option, Func< Option<T>> f) =>
        (await option).Else(f);

    public static async Task<Option<T>> Else<T>(this Task<Option<T>> option, Func<Task<Option<T>>> f) =>
        await (await option).Else(f);

    //-------------------------------------------------------------------------- Else/Void

    public static async Task Else<T>(this Task<Option<T>> option, Action f) =>
        (await option).Else(f);

    public static async Task Else<T>(this Task<Option<T>> option, Func<Task> f) =>
        await (await option).Else(f);

    //========================================================================== Adapter Left

    //-------------------------------------------------------------------------- Filter

    public static async Task<Option<T>> Filter<T>(this Task<Option<T>> option, Func<T,bool> predicate) => 
        (await option).Filter(predicate);

    //========================================================================== Combinator Left

    //-------------------------------------------------------------------------- Or

    public static async Task<Option<T>> Or<T>(this Task<Option<T>> option, Option<T> other) => 
        (await option).Or(other);

    public static async Task<Option<T>> Or<T>(this Task<Option<T>> option, Func<Option<T>> other) => 
        (await option).Or(other);

    public static async Task<Option<T>> Or<T>(this Task<Option<T>> option, Func<Task<Option<T>>> other) => 
        await (await option).Or(other);

    //-------------------------------------------------------------------------- And

    public static async Task<Option<U>> And<T,TOther,U>(this Task<Option<T>> option, Option<TOther> other, Func<T,TOther,U> selector) => 
        (await option).And(other, selector);

    public static async Task<Option<U>> And<T,TOther,U>(this Task<Option<T>> option, Func<Option<TOther>> other, Func<T,TOther,U> selector) => 
        (await option).And(other, selector);

    public static async Task<Option<U>> And<T,TOther,U>(this Task<Option<T>> option, Func<Task<Option<TOther>>> other, Func<T,TOther,U> selector) => 
        await (await option).And(other, selector);
}
