namespace FunctionalLink.Next;

#pragma warning disable 1591
#pragma warning disable 1998

public class Either<V,O>
{
    internal readonly int flag;
    internal readonly V value;
    internal readonly O other;

    protected Either(int flag, V value, O other) =>
        (this.flag, this.value, this.other) = (flag, value, other);

    public static Either<V,O> Value(V value) =>
        new(1, value, default);

    public static Either<V,O> Other(O failure) =>
        new(2, default, failure);

    public static implicit operator Either<V,O>(Value<V> value) =>
        Value(value.Value_);

    public static implicit operator Either<V,O>(Other<O> other) =>
        Other(other.Value);

    //========================================================================== Inspect

    public bool HasValue() => 
        flag == 1;
    
    public bool HasOther() => 
        flag == 2;

    //========================================================================== Decompose 

    //-------------------------------------------------------------------------- Has

    public bool HasValue(out V value) 
    {
        value = this.value;
        return flag == 1;
    }

    public bool HasOther(out O other) 
    {
        other = this.other;
        return flag == 2;
    }

    //-------------------------------------------------------------------------- Match

    /// <summary>
    /// Match -> Void
    /// </summary>
    public void Match(Action<V> onValue, Action<O> onOther) 
    {
        if (flag == 1)
            onValue(value);
        else
            onOther(other);
    }

    /// <summary>
    /// Match -> Task
    /// </summary>
    public async Task Match(Func<V,Task> onValue, Func<O,Task> onOther) 
    {
        if (flag == 1)
            await onValue(value);
        else
            await onOther(other);
    }

    /// <summary>
    /// Match -> U
    /// </summary>
    public U Match<U>(Func<V,U> onValue, Func<O,U> onOther) =>
        flag == 1 
            ? onValue(value) 
            : onOther(other);

    //-------------------------------------------------------------------------- ValueOr

    public V ValueOr(V alternate) =>
        Match(
            value => value,
            _ => alternate);

    public V ValueOr(Func<V> alternate) =>
        Match(
            value => value,
            _ => alternate());

    public Task<V> ValueOr(Func<Task<V>> alternate) =>
        Match(
            value => Task.FromResult(value),
            _ => alternate());

    //========================================================================== Compose Left

    //-------------------------------------------------------------------------- Then/Map

    /// <summary>
    /// Map/Left -> T
    /// </summary>
    public Either<U,O> Then<U>(Func<V,U> f) =>
        Match(
            value => Either<U,O>.Value(f(value)), 
            other => Either<U,O>.Other(other));   

    /// <summary>
    /// Map/Left -> T Async;
    /// </summary>
    public async Task<Either<U,O>> Then<U>(Func<V,Task<U>> f) =>
        await Match(
            async value => Either<U,O>.Value(await f(value)), 
            async other => Either<U,O>.Other(other));                

    //-------------------------------------------------------------------------- Then/Bind

    /// <summary>
    /// Bind/Left -> Result
    /// </summary>
    public Either<U,O> Then<U>(Func<V,Either<U,O>> f) =>
        Match(
            value => f(value).Match(
                value1 => Either<U,O>.Value(value1),
                other1 => Either<U,O>.Other(other1)), 
            other => Either<U,O>.Other(other));

    /// <summary>
    /// Bind/Left -> Result Async
    /// </summary>
    public async Task<Either<U,O>> Then<U>(Func<V,Task<Either<U,O>>> f) =>
        await Match(
            async value => (await f(value)).Match(
                value1 => Either<U,O>.Value(value1),
                other1 => Either<U,O>.Other(other1)), 
            async other => Either<U,O>.Other(other));

    //-------------------------------------------------------------------------- Then/Void

    /// <summary>
    /// Void/Left -> Result
    /// </summary>
    public Either<V,O> Then(Action<V> f)
    {
        Match(
            value => { f(value); },
            other => { });
        return this;
    }

    /// <summary>
    /// Void/Left -> Result Async
    /// </summary>
    public async Task<Either<V,O>> Then(Func<V, Task> f)
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
    public Either<V,O> Else(Func<O,V> f) =>
        Match(
            value => Either<V,O>.Value(value), 
            other => Either<V,O>.Value(f(other)));   

    /// <summary>
    /// Map/Right -> T Async
    /// </summary>
    public async Task<Either<V,O>> Else(Func<O,Task<V>> f) =>
        await Match(
            async value => Either<V,O>.Value(value), 
            async other => Either<V,O>.Value(await f(other)));        

    //-------------------------------------------------------------------------- Else/Bind

    /// <summary>
    /// Bind/Right -> Result
    /// </summary>
    public Either<V,O> Else(Func<O, Either<V,O>> f) =>
        Match(
            _ => this, 
            other => f(other));   

    /// <summary>
    /// Bind/Right -> Result Async
    /// </summary>
    public async Task<Either<V,O>> Else(Func<O,Task<Either<V,O>>> f) =>
        await Match(
            async _ => this, 
            async other => await f(other));        

    //-------------------------------------------------------------------------- Else/Void

    /// <summary>
    /// Void/Right -> Void
    /// </summary>
    public Either<V,O> Else(Action<O> f) 
    {
        Match(
            _ => {}, 
            other => f(other));   
        return this;
    }

    /// <summary>
    /// Void/Right -> Task
    /// </summary>
    public async Task<Either<V,O>> Else(Func<O,Task> f) 
    {
        await Match(
            async _ => {}, 
            async other => await f(other));        
        return this;
    }

    //========================================================================== Adapter Left

    //-------------------------------------------------------------------------- Filter

    public Either<V,O> Filter(Func<V,bool> predicate, O other) => 
        Match(
            success => predicate(success) ? this : Other(other),
            failure => this);
        
    public Either<V,O> Filter(Func<V,bool> predicate, Func<V,O> other) => 
        Match(
            success => predicate(success) ? this : Other(other(success)),
            failure => this);

    //========================================================================== Combinator Left

    //-------------------------------------------------------------------------- Or

    public Either<V,O> Or(Either<V,O> other) => 
        Match(
            success => this,
            failure => other);

    public Either<V,O> Or(Func<Either<V,O>> other) => 
        Match(
            success => this,
            failure => other());

    public Task<Either<V,O>> Or(Func<Task<Either<V,O>>> other) => 
        Match(
            success => Task.FromResult(this),
            failure => other());

    //-------------------------------------------------------------------------- And

    public Either<U,O> And<TOther,U>(Either<TOther,O> other, Func<V,TOther, U> selector) => 
        Match(
            success => other.Match(
                success1 => Either<U,O>.Value(selector(success, success1)),
                failure1 => Either<U,O>.Other(failure1)),
            failure => Either<U,O>.Other(failure));

    public Either<U,O> And<TOther,U>(Func<Either<TOther,O>> other, Func<V,TOther,U> selector) => 
        Match(
            success => (other()).Match(
                success1 => Either<U,O>.Value(selector(success, success1)),
                failure1 => Either<U,O>.Other(failure1)),
            failure => Either<U,O>.Other(failure));

    public async Task<Either<U,O>> And<TOther,U>(Func<Task<Either<TOther,O>>> other, Func<V,TOther,U> selector) => 
        await Match(
            async success => (await other()).Match(
                success1 => Either<U,O>.Value(selector(success, success1)),
                failure1 => Either<U,O>.Other(failure1)),
            async failure => Either<U,O>.Other(failure));
}

