namespace FunctionalLink.Next;

public static partial class Option
{
    /// <summary>
    /// This is an alternate constructor for creating an Option in a Some state.
    /// </summary>
    /// <param name="value">The target value</param>
    /// <typeparam name="T">The type of the target value</typeparam>
    /// <returns>An Option in a Some state</returns>
    /// <remarks>
    /// This class enables you to create Options like so:
    /// <code>
    /// public static Option&lt;int&gt; Divide(int a, int b) =>
    ///     b != 0
    ///         ? Option.Some(a / b)
    ///         : Option.None();
    /// </code>
    /// These are all equivalent constructors:
    /// <code>
    /// Some(123);
    /// Option.Some(123);
    /// Option&lt;int&gt;.Some(123);
    /// </code>
    /// </remarks>
    public static Option<T> Some<T>(T value) =>
        Option<T>.Some(value);

    /// <summary>
    /// This is an alternate constructor for creating an Option from an unknown nullable value.
    /// </summary>
    /// <param name="value">The target value</param>
    /// <typeparam name="T">The target value type</typeparam>
    /// <returns>An Option in the Some state if value is not null.  Otherwise in the None state</returns>
    /// These are all equivalent constructors:
    /// <code>
    /// value.Maybe();
    /// Maybe(value);
    /// Option.Maybe(value);
    /// Option&lt;int&gt;.Maybe(value);
    /// </code>
    public static Option<T> Maybe<T>(T value) =>
        Option<T>.Maybe(value);
}
