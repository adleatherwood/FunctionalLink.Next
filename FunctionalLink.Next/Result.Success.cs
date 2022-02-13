namespace FunctionalLink.Next;

/// <summary>
/// A generic success wrapper that is implicitly convertable to a Result
/// </summary>
/// <typeparam name="T"></typeparam>
public class Success<T>
{
    internal Success(T value) =>
        Value = value;

    internal readonly T Value;
}

public static partial class Result
{
    /// <summary>
    /// Creates a Success&lt;T&gt; with the given value.  The Success&lt;T&gt; type
    /// can be implicitly converted to Result&lt;T&gt; or a Result&lt;T, _&gt;  
    /// </summary>
    /// <param name="value">The value to assign</param>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <returns></returns>
    public static Success<T> Success<T>(T value) =>
        new (value);

    /// <summary>
    /// Creates a Success&lt;T&gt; with None as the value.  The Success&lt;T&gt; type
    /// can be implicitly converted to Result&lt;T&gt; or a Result&lt;T, _&gt;.  This
    /// is useful when you succeeded but have no value to return.
    /// </summary>
    /// <returns></returns>
    public static Success<None> Success() =>
        new (None.Value);
}
