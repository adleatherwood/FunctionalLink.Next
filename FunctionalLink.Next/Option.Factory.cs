namespace FunctionalLink.Next;

#pragma warning disable 1591

public static class Option
{  
    public static Option<T> Maybe<T>(T value) 
        where T : class =>
        value != null 
            ? Some(value)
            : None();

    public static Option<T> Some<T>(T value) =>
        Option<T>.Some(value);
        
    public static None None() =>
        Next.None.Value;
}
