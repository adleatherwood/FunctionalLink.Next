namespace FunctionalLink.Next;

/// <summary>
/// A few extra methods for composing with Linq.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Executes an Action on each item in an IEnumerable.  
    /// </summary>
    /// <param name="enumerable">The target enumerable</param>
    /// <param name="action">The action to perform</param>
    /// <returns>The item from the collection unmodified</returns>
    /// <remarks>
    /// An example:
    /// <code>
    /// values
    ///     .Iterate(Console.WriteLine)
    ///     .ToList();
    /// </code>
    /// </remarks>
    public static IEnumerable<T> Iterate<T>(this IEnumerable<T> enumerable, Action<T> action) =>
        enumerable.Select(i =>
        {
            action(i);
            return i;
        });

    /// <summary>
    /// Forces the evaluation of an IEnumerable to a materialized collection that will not reevaluate.
    /// </summary>
    /// <param name="enumerable">The target enumerable</param>
    /// <returns>A collection that will not reevaluate</returns>
    public static IReadOnlyCollection<T> Evaluate<T>(this IEnumerable<T> enumerable) =>
        enumerable.ToList();

    /// <summary>
    /// Forces the evaluation of an IEnumerable and ignores the result.  This can be useful when composing
    /// a series of actions to occur, but not needing a result to return.
    /// </summary>
    /// <param name="enumerable">The target enumerable</param>
    public static void EvaluateAndIgnore<T>(this IEnumerable<T> enumerable) =>
        enumerable.Evaluate().Ignore();
}
