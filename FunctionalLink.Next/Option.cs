namespace FunctionalLink.Next;

#pragma warning disable 1591
#pragma warning disable 1998

public class Option<T>
{
    internal readonly int flag;
    internal readonly T some;
    
    protected Option(int flag, T some) =>
        (this.flag, this.some) = (flag, some);

    public static Option<T> Some(T some) =>
        new(1, some);

    public static Option<T> None() =>
        new(2, default);

    public static implicit operator Option<T>(T some) =>
        Some(some);

    public static implicit operator Option<T>(None none) =>
        None();

    //========================================================================== Inspect

    public bool HasSome() => 
        flag == 1;
    
    public bool HasNone() => 
        flag == 2;

    //========================================================================== Decompose 

    //-------------------------------------------------------------------------- Has

    public bool HasSome(out T some) 
    {
        some = this.some;
        return flag == 1;
    }

    //-------------------------------------------------------------------------- Match

    /// <summary>
    /// Match -> Void
    /// </summary>
    public void Match(Action<T> onSome, Action onNone) 
    {
        if (flag == 1)
            onSome(some);
        else
            onNone();
    }

    /// <summary>
    /// Match -> Task
    /// </summary>
    public async Task Match(Func<T,Task> onSome, Func<Task> onNone) 
    {
        if (flag == 1)
            await onSome(some);
        else
            await onNone();
    }

    /// <summary>
    /// Match -> U
    /// </summary>
    public U Match<U>(Func<T,U> onSome, Func<U> onNone) =>
        flag == 1 
            ? onSome(some) 
            : onNone();

    //-------------------------------------------------------------------------- ValueOr

    public T ValueOr(T alternate) =>
        Match(
            some => some,
            () => alternate);

    public T ValueOr(Func<T> alternate) =>
        Match(
            some => some,
            () => alternate());

    public Task<T> ValueOr(Func<Task<T>> alternate) =>
        Match(
            some => Task.FromResult(some),
            () => alternate());

    //========================================================================== Compose Left

    //-------------------------------------------------------------------------- Then/Map

    /// <summary>
    /// Map/Left -> T
    /// </summary>
    public Option<U> Then<U>(Func<T,U> f) =>
        Match(
            some => Option<U>.Some(f(some)), 
            () => Option<U>.None());   

    /// <summary>
    /// Map/Left -> T Async;
    /// </summary>
    public async Task<Option<U>> Then<U>(Func<T,Task<U>> f) =>
        await Match(
            async some => Option<U>.Some(await f(some)), 
            async () => Option<U>.None());                

    //-------------------------------------------------------------------------- Then/Bind

    /// <summary>
    /// Bind/Left -> Option
    /// </summary>
    public Option<U> Then<U>(Func<T,Option<U>> f) =>
        Match(
            some => f(some).Match(
                some1 => Option<U>.Some(some1),
                () => Option<U>.None()), 
            () => Option<U>.None());

    /// <summary>
    /// Bind/Left -> Option Async
    /// </summary>
    public async Task<Option<U>> Then<U>(Func<T,Task<Option<U>>> f) =>
        await Match(
            async some => (await f(some)).Match(
                some1 => Option<U>.Some(some1),
                () => Option<U>.None()), 
            async () => Option<U>.None());

    //-------------------------------------------------------------------------- Then/Void

    /// <summary>
    /// Void/Left -> Option
    /// </summary>
    public Option<T> Then(Action<T> f)
    {
        Match(
            some => { f(some); },
            () => { });
        return this;
    }

    /// <summary>
    /// Void/Left -> Option Async
    /// </summary>
    public async Task<Option<T>> Then(Func<T, Task> f)
    {
        await Match(
            async some => { await f(some); },
            async () => { });
        return this;
    }

    //========================================================================== Compose Right

    //-------------------------------------------------------------------------- Else/Map
    
    /// <summary>
    /// Map/Right -> T
    /// </summary>
    public Option<T> Else(Func<T> f) =>
        Match(
            some => Option<T>.Some(some), 
            () => Option<T>.Some(f()));   

    /// <summary>
    /// Map/Right -> T Async
    /// </summary>
    public async Task<Option<T>> Else(Func<Task<T>> f) =>
        await Match(
            async some => Option<T>.Some(some), 
            async () => Option<T>.Some(await f()));        

    //-------------------------------------------------------------------------- Else/Bind

    /// <summary>
    /// Bind/Right -> Option
    /// </summary>
    public Option<T> Else(Func<Option<T>> f) =>
        Match(
            _ => this, 
            () => f());   

    /// <summary>
    /// Bind/Right -> Option Async
    /// </summary>
    public async Task<Option<T>> Else(Func<Task<Option<T>>> f) =>
        await Match(
            async _ => this, 
            async () => await f());        

    //-------------------------------------------------------------------------- Else/Void

    /// <summary>
    /// Void/Right -> Void
    /// </summary>
    public void Else(Action f) =>
        Match(
            _ => {}, 
            () => f());   

    /// <summary>
    /// Void/Right -> Task
    /// </summary>
    public async Task Else(Func<Task> f) =>
        await Match(
            async _ => {}, 
            async () => await f());        

    //========================================================================== Adapter Left

    //-------------------------------------------------------------------------- Filter

    public Option<T> Filter(Func<T,bool> predicate) => 
        Match(
            some => predicate(some) ? this : None(),
            () => this);
        
    //========================================================================== Combinator Left

    //-------------------------------------------------------------------------- Or

    public Option<T> Or(Option<T> other) => 
        Match(
            some => this,
            () => other);

    public Option<T> Or(Func<Option<T>> other) => 
        Match(
            some => this,
            () => other());

    public Task<Option<T>> Or(Func<Task<Option<T>>> other) => 
        Match(
            _  => Task.FromResult(this),
            () => other());

    //-------------------------------------------------------------------------- And

    public Option<U> And<TOther,U>(Option<TOther> other, Func<T,TOther, U> selector) => 
        Match(
            some => other.Match(
                some1 => Option<U>.Some(selector(some, some1)),
                () => Option<U>.None()),
            () => Option<U>.None());

    public Option<U> And<TOther,U>(Func<Option<TOther>> other, Func<T,TOther,U> selector) => 
        Match(
            some => other().Match(
                some1 => Option<U>.Some(selector(some, some1)),
                () => Option<U>.None()),
            () => Option<U>.None());

    public async Task<Option<U>> And<TOther,U>(Func<Task<Option<TOther>>> other, Func<T,TOther,U> selector) => 
        await Match(
            async some => (await other()).Match(
                some1 => Option<U>.Some(selector(some, some1)),
                () => Option<U>.None()),
            async () => Option<U>.None());
}

