namespace FunctionalLink.Next;

/// <summary>
/// A failure wrapper that is implicitly convertable to a Result.  This type is not intended
/// to be constructed manually.  It exists entirely, to reduce the amount of code required
/// to construct Results.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Failure<T>
{
    internal Failure(T value) =>
        Value = value;

    internal readonly T Value;
}

/// <summary>
/// A static class for constructing Results in a more explicit way, if desired.
/// </summary>
public static partial class Result
{
    /// <summary>
    /// Creates a Failure&lt;T&gt; with the given value.  The Failure&lt;T&gt; type
    /// can be implicitly converted to Result&lt;T&gt; or a Result&lt;T, _&gt;
    /// </summary>
    /// <param name="value">The failure value</param>
    /// <typeparam name="T">The type of the failure</typeparam>
    /// <returns>A Failure state that is implicitly convertible to a Result</returns>
    public static Failure<T> Failure<T>(T value) =>
        new (value);
}
