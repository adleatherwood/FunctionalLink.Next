namespace FunctionalLink.Next;

#pragma warning disable 1591

public static class GlobalLink
{    
    public static T Self<T>(T value) =>
        value;
 
    public static Option<T> Some<T>(T value) =>
        Option<T>.Some(value);
    
    public static None None() =>
        Next.None.Value;
    
    public static Option<T> Maybe<T>(T? value) 
        where T : class =>
        Option<T>.Maybe(value);
    
    public static Result<T> Success<T>(T value) =>
        Result<T>.Success(value);

    public static Error Failure(string failure) =>
        new(failure); 

    public static Error Failure(Exception failure) =>
        new(failure); 
}
