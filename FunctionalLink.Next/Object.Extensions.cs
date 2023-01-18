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
    /// Used to explicitly declare a value as unused.
    /// </summary>
    /// <param name="obj">The value to ignore</param>
    public static async Task Ignore<T>(this Task<T> obj)
    {
        await obj;
    }
}
