namespace FunctionalLink.Next;

/// <summary>
/// A few extensions that aid in functional composition.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Turns any object into an IEnumerable with the given value as it's single element.
    /// </summary>
    /// <param name="obj">The object to add to a collection</param>
    /// <returns>An IEnumerable with a single element</returns>
    public static IEnumerable<T> Enumerate<T>(this T obj)
    {
        yield return obj;
    }
    
    /// <summary>
    /// Used to explicitly declare a value as unused.
    /// </summary>
    /// <param name="obj">The value to ignore</param>
    public static void Ignore(this object obj)
    {
    }
    
    /// <summary>
    /// Creates an Option from an unknown source where a null value could be expected.  A null value
    /// will be interpreted as None.  Otherwise, the Option will be in a Some state with the given value.
    /// </summary>
    /// <param name="value">The unknown target</param>
    /// <returns>An Option in a Some state if the value is not null.  Otherwise, in a None state</returns>
    public static Option<T> Maybe<T>(this T value) =>
        Option.Maybe(value);
}
