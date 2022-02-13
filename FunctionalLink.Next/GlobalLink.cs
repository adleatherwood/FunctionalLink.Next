namespace FunctionalLink.Next;

/// <summary>
/// If enabled, this class provides constructors for all of the types in the library without needing to
/// fully qualify the type.  Using this is completely optional, but does create very clean looking code.
/// </summary>
/// <remarks>
/// To enable this feature, make a static using statement like so:
///
/// <code>
/// using static FunctionalLink.Next.GlobalLink;
/// </code>
///
/// Once enabled, you can construct Options and Results with less code.  e.g.:
/// <code>
/// return value != null
///     ? Some(value); 
///     : None();
/// 
/// return found
///     ? Success(value); 
///     : Failure("Value not found"); 
/// </code> 
/// </remarks>
public static class GlobalLink
{
    /// <summary>
    /// A self referencing function.  A function that returns the entity passed to it.
    /// </summary>
    /// <param name="value">The target value</param>
    /// <typeparam name="T">The type of the target value</typeparam>
    /// <returns>The given value unmodified</returns>
    /// <remarks>
    /// This is often useful in Linq statements.  You can replace this:
    /// <code>
    /// var result = values.GroupBy(i => i);
    /// </code>
    /// with
    /// <code>
    /// var result = values.GroupBy(Self);
    /// </code>
    /// It's largely an aesthetic choice.  But technically you are not reimplementing (i => i) over and over.  This
    /// is also positive.
    /// </remarks>
    public static T Self<T>(T value) =>
        value;

    /// <summary>
    /// This is an alternate constructor for creating an Option in a Some state.
    /// </summary>
    /// <param name="value">The target value</param>
    /// <typeparam name="T">The target value type</typeparam>
    /// <returns>An Option in a Some state</returns>
    /// <remarks>
    /// These are equivalent constructors:
    /// <code>
    /// Some(123);
    /// Option.Some(123);
    /// Option&lt;int&gt;.Some(123);
    /// </code>
    /// </remarks>
    public static Option<T> Some<T>(T value) =>
        Option<T>.Some(value);
    
    /// <summary>
    /// This is an alternate constructor for creating an Option in a None state.
    /// </summary>
    /// <returns>An Option in a None state</returns>
    /// <remarks>
    /// These are equivalent constructors:
    /// <code>
    /// None();
    /// Option.None();
    /// Option&lt;int&gt;.None();
    /// </code>
    /// </remarks>
    public static None None() =>
        Next.None.Value;

    /// <summary>
    /// This is an alternate constructor for creating an Option from an unknown nullable value.
    /// </summary>
    /// <param name="value">The target value</param>
    /// <typeparam name="T">The target value type</typeparam>
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
    public static Option<T> Maybe<T>(T value) =>
        Option.Maybe(value);

    /// <summary>
    /// This is an alternate constructor for a Result in a success state. 
    /// </summary>
    /// <param name="value">The target value</param>
    /// <typeparam name="T">The target value type</typeparam>
    /// <returns>A Success that implicitly converts to a Result</returns>
    /// <remarks>
    /// The implicit conversion allows you to declare a successful result with less type information.  e.g.
    /// <code>
    /// public static Result&lt;int&gt; Divide(int a, int b) =>
    ///     return b != 0
    ///         ? Success(a / b)
    ///         : Failure("Cannot divide by 0");
    /// </code>
    /// </remarks>
    public static Success<T> Success<T>(T value) =>
        new(value);

    /// <summary>
    /// An alternate constructor for a Result in a success state, but with no particular value to return.
    /// </summary>
    /// <returns>A Success of None that implicitly converts to a Result</returns>
    public static Success<None> Success() =>
        new(Next.None.Value);
    
    /// <summary>
    /// This is an alternate constructor for a Result in a failure state.
    /// </summary>
    /// <param name="failure">The failure value</param>
    /// <typeparam name="T">The failure value type</typeparam>
    /// <returns>A Failure that implicitly converts to a Result</returns>
    /// <remarks>
    /// The implicit conversion allows you to declare a successful result with less type information.  e.g.
    /// <code>
    /// public static Result&lt;int&gt; Divide(int a, int b) =>
    ///     return b != 0
    ///         ? Success(a / b)
    ///         : Failure("Cannot divide by 0");
    /// </code>
    /// Without the implicit conversion, the code would look like this:
    /// <code>
    /// public static Result&lt;int&gt; Divide(int a, int b) =>
    ///     return b != 0
    ///         ? Option&lt;int&gt;.Success(a / b)
    ///         : Option&lt;int&gt;.Failure("Cannot divide by 0");
    /// </code>
    /// This is purely an aesthetic choice.  Both implementations yield the same result.
    /// </remarks>
    public static Failure<T> Failure<T>(T failure) =>
        new (failure);
}
