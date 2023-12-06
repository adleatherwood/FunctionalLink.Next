namespace FunctionalLink.Next;

#pragma warning disable 1591

public class Result
    : Result<None>
{
    protected Result(int flag, Error failure) : base(flag, None.Value, failure)
    {
    }

    public static implicit operator Result(Exception failure) =>
        new Result(2, new Error(failure));

    public static implicit operator Result(Error failure) =>
        new Result(2, failure);

    public static Result Success() =>
        new Result(1, default!);

    public static Result<T> Success<T>(T value) =>
        Result<T>.Success(value);

    public new static Error Failure(string error) =>
        new Error(error);

    public new static Error Failure(Exception error) =>
        new Error(error);
    
    public static Error Failure(string message, Exception error) =>
        new Error(message, error);
}
