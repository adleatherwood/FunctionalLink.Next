namespace FunctionalLink.Next;

#pragma warning disable 1591

public static class Result
{    
    public static Result<T> Success<T>(T value) =>
        Result<T>.Success(value);

    public static Error Failure(string error) =>
        new Error(error);

    public static Error Failure(Exception error) =>
        new Error(error);
    
    public static Error Failure(string message, Exception error) =>
        new Error(message, error);
}
