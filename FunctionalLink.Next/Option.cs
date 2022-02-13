namespace FunctionalLink.Next;

/// <summary>
/// The Option class works like a Nullable&lt;T&gt;, but for reference types as well as
/// primitive types.  It is intended to replace the use of null as a value.  
/// </summary>
/// <typeparam name="T">The type of the optional value</typeparam>
/// <remarks>
///
/// As an example, the following code:
///
/// <code>
/// public static string ReadFile(string filename) =>
///     File.Exists(filename)
///         ? File.ReadAllText(filename)
///         : null;
/// </code>
///
/// Can be rewritten as:
/// 
/// <code>
/// public static Option&lt;string&gt; ReadFile(string filename) =>
///     File.Exists(filename)
///         ? Some(File.ReadAllText(filename))
///         : None();
/// </code>
///
/// The idea here is that the method signature is telling you explicitly that a return value
/// might not be possible here, rather than returning an unexpected null.
/// </remarks>
public class Option<T>
{
    private Option(bool hasValue, T value) =>
        (_hasValue, _value) = (hasValue, value);
    
    private readonly bool _hasValue;
    private readonly T _value;
    
    /// <summary>
    /// The constructor for an Option that contains a value.
    /// </summary>
    /// <param name="value">The value to contain</param>
    /// <returns>An Option in the Some state with the given value</returns>
    /// <remarks>
    /// These are equivalent constructors:
    /// <code>
    /// Some(123);
    /// Option.Some(123);
    /// Option&lt;int&gt;.Some(123);
    /// </code>
    /// </remarks>
    public static Option<T> Some(T value) =>
        new (true, value);
    
    /// <summary>
    /// The constructor for an Option with no value.
    /// </summary>
    /// <returns>An Option in the None state</returns>
    /// <remarks>
    /// These are equivalent constructors:
    /// <code>
    /// None();
    /// Option.None();
    /// Option&lt;int&gt;.None();
    /// </code>
    /// </remarks>
    public static Option<T> None() =>
        new (false, default!);
    
    /// <summary>
    /// This is an alternate constructor for creating an Option from an unknown nullable value.
    /// </summary>
    /// <param name="value">The target value</param>
    /// <returns>An Option in the Some state if value is not null.  Otherwise in the None state</returns>
    /// <remarks>
    /// These are equivalent constructors:
    /// <code>
    /// value.Maybe();
    /// Maybe(value);
    /// Option.Maybe(value);
    /// Option&lt;int&gt;.Maybe(value);
    /// </code>
    /// </remarks>
    public static Option<T> Maybe(T value) =>
        value != null 
            ? Some(value) 
            : None();

    /// <summary>
    /// An implicit conversion operator used to make constructing Options require less
    /// syntax.
    /// </summary>
    /// <param name="none">The None value to convert</param>
    /// <returns>An Option in a None state</returns>
    /// <remarks>
    /// Typically, this operator is used like so:
    /// <code>
    /// public static Option&lt;int&gt; Divide(int a, int b) =>
    ///     b != 0
    ///         ? Some(a / b)
    ///         : None(); // this None is implicitly converted to Option&lt;int&gt;  
    /// </code>
    /// To avoid the implicit conversion, use the 'None' Option constructor directly:
    /// <code>
    /// public static Option&lt;int&gt; Divide(int a, int b) =>
    ///     b != 0
    ///         ? Option&lt;int&gt;.Some(a / b)
    ///         : Option&lt;int&gt;.None(); // no implicit conversion is done here  
    /// </code>
    /// </remarks>
    public static implicit operator Option<T>(None none) =>
        new (false, default!);

    /// <summary>
    /// Used as a way to deconstruct an Option and retrieve the internal value, if it exists.
    /// </summary>
    /// <param name="value">The internal value of the Option</param>
    /// <returns>True if the Option contains a value. Otherwise false</returns>
    /// <remarks>
    /// For example:
    /// <code>
    /// var user = FindUser(userId); // returns an Option
    /// if (user.HasValue(out var value))
    /// {
    ///     ... // do something
    /// } 
    /// </code>
    /// </remarks>
    public bool HasValue(out T value)
    {
        value = _hasValue ? _value : default!;
        return _hasValue;
    }

    /// <summary>
    /// Another way to deconstruct an Option and run an specific Action for each state it
    /// may be in.  If the Option contains a value, onSome is executed.  Otherwise, onNone
    /// is executed.
    /// </summary>
    /// <param name="onSome">The action to execute if the Option contains a value</param>
    /// <param name="onNone">The action to execute if the Option does not contain a value</param>
    /// <remarks>
    /// <code>
    /// var user = FindUser(userId); // returns an Option
    /// user.Match(
    ///     some => Console.WriteLine($"Hello {some}"),
    ///     none => Console.WriteLine("User not found!"));
    /// </code>
    /// </remarks>
    public void Match(Action<T> onSome, Action<None> onNone)
    {
        if (HasValue(out var value))
            onSome(value);
        else
            onNone(Next.None.Value);
    }

    /// <summary>
    /// Another way to deconstruct an Option and run an specific Func for each state it
    /// may be in.  If the Option contains a value, onSome is executed.  Otherwise, onNone
    /// is executed.  Both functions must have the same return type.
    /// </summary>
    /// <param name="onSome">The function to execute if the Option contains a value</param>
    /// <param name="onNone">The function to execute if the Option does not contain a value</param>
    /// <typeparam name="TResult">The type of the result returned from both functions</typeparam>
    /// <returns>The result of onSome or onNone depending on the state of the Option</returns>
    /// <remarks>
    /// <code>
    /// var user = FindUser(userId); // returns an Option
    /// var username = user.Match(
    ///     some => some.Username),
    ///     none => "Mystery Person");
    /// 
    /// Console.WriteLine($"Hello {username}");
    /// </code>
    /// </remarks>
    public TResult Match<TResult>(Func<T, TResult> onSome, Func<None, TResult> onNone) =>
        HasValue(out var value)
            ? onSome(value)
            : onNone(Next.None.Value);

    /// <summary>
    /// If the Option contains a value, then the given function will be executed and the result
    /// will be passed to the next function.  This overload is akin to 'Bind' or 'SelectMany' in
    /// other programming contexts. 
    /// </summary>
    /// <param name="f">The function to execute if the Option contains a value</param>
    /// <typeparam name="TResult">The type of the result returned from the given function</typeparam>
    /// <returns>An Option of the new type of TResult</returns>
    public Option<TResult> Then<TResult>(Func<T, Option<TResult>> f) =>
        HasValue(out var value)
            ? f(value)
            : Option<TResult>.None();

    /// <summary>
    /// If the Option contains a value, then the given function will be executed and the result
    /// will be passed to the next function.  This overload is akin to 'Map' or 'Select' in
    /// other programming contexts. 
    /// </summary>
    /// <param name="f">The function to execute if the Option contains a value</param>
    /// <typeparam name="TResult">The type of the result returned from the given function</typeparam>
    /// <returns>An Option of the new type of TResult</returns>
    public Option<TResult> Then<TResult>(Func<T, TResult> f) =>
        HasValue(out var value)
            ? Option<TResult>.Some(f(value))
            : Option<TResult>.None();

    /// <summary>
    /// If the Option contains a value, then the given action will be executed.  
    /// </summary>
    /// <param name="f">The Action to execute if the Option contains a value</param>
    /// <returns>The Option returns itself in this case</returns>
    public Option<T> Then(Action<T> f)
    {
        if (HasValue(out var value))
            f(value);

        return this;
    }
    
    /// <summary>
    /// Used to transform a None state into Some with the alternate value.
    /// </summary>
    /// <param name="alt">The alternate value</param>
    /// <returns>If the current Option is None, then the alternate value is applied</returns>
    public Option<T> Else(T alt) =>
        _hasValue
            ? this
            : Some(alt);

    /// <summary>
    /// Used to transform a None state into Some with the alternate value.
    /// </summary>
    /// <param name="alt">The function to provide an alternate value</param>
    /// <returns>If the current Option is None, then the alternate value is applied</returns>
    public Option<T> Else(Func<T> alt) =>
        _hasValue
            ? this
            : Some(alt());
    
    /// <summary>
    /// A way of deconstructing an Option and retrieving the value contained with, if it exists.
    /// If a value exists, it is return.  Otherwise, the alternate value is returned.
    /// </summary>
    /// <param name="alt">The alternate value to return if the Option does not contain a value</param>
    /// <returns>Either the value within the Option or the alternate value</returns>
    public T ValueOr(T alt) =>
        HasValue(out var value)
            ? value
            : alt;

    /// <summary>
    /// Provides a way of chaining two Options together returning whichever Option has a value.
    /// If a value exists in current Option, the current Option is carried forward.  Otherwise,
    /// the alternate Option is carried forward. 
    /// </summary>
    /// <param name="alt">The alternate Option to use if the current Option has no value</param>
    /// <returns>Either the current Option or the alternate Option based on their states</returns>
    public Option<T> Or(Option<T> alt) =>
        _hasValue
            ? this
            : alt;
    
    /// <summary>
    /// Provides a way of chaining two Options together returning whichever Option has a value.
    /// If a value exists in current Option, the current Option is carried forward.  Otherwise,
    /// the alternate Option is carried forward.
    ///
    /// In this version, the alternate Option is evaluated lazily via the given function.
    /// </summary>
    /// <param name="alt">The alternate Option to use if the current Option has no value</param>
    /// <returns>Either the current Option or the alternate Option based on their states</returns>
    public Option<T> Or(Func<Option<T>> alt) =>
        _hasValue
            ? this
            : alt();

    /// <summary>
    /// Provides a way to combine two Options with values together into a new type.  This is
    /// akin to 'Join' in Linq.
    /// </summary>
    /// <param name="other">Another Option to combine with the current Option</param>
    /// <param name="selector">The function that combines the two values together and produces a new value</param>
    /// <typeparam name="TOther">The type of the other Option</typeparam>
    /// <typeparam name="TResult">The type of the new TOption</typeparam>
    /// <returns>An Option of the new result type</returns>
    public Option<TResult> And<TOther, TResult>(Option<TOther> other, Func<T, TOther, TResult> selector)
    {
        if (!_hasValue)
            return Option<TResult>.None();

        return other.HasValue(out var value)
            ? Option<TResult>.Some(selector(_value, value))
            : Option<TResult>.None();
    }
    
    /// <summary>
    /// Provides a way to combine two Options with values together into a new type.  This is
    /// akin to 'Join' in Linq.
    ///
    /// In this overload, the other Option is evaluated lazily.
    /// </summary>
    /// <param name="other">Another Option to combine with the current Option</param>
    /// <param name="selector">The function that combines the two values together and produces a new value</param>
    /// <typeparam name="TOther">The type of the other Option</typeparam>
    /// <typeparam name="TResult">The type of the new Option</typeparam>
    /// <returns>An Option of the new result type</returns>
    public Option<TResult> And<TOther, TResult>(Func<Option<TOther>> other, Func<T, TOther, TResult> selector)
    {
        if (!_hasValue)
            return Option<TResult>.None();

        var bb = other();

        return bb.HasValue(out var value)
            ? Option<TResult>.Some(selector(_value, value))
            : Option<TResult>.None();
    }
}
