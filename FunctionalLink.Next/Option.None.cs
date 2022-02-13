namespace FunctionalLink.Next;

/// <summary>
/// None is used to declare a None state for an Option.  This type is implicitly convertible to an Option.
/// </summary>
public class None
{
    private None()
    {
    }

    internal static readonly None Value = new None();
}

/// <summary>
/// A static class for constructing Options in a more explicit way, if desired.
/// </summary>
public static partial class Option
{
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
}
