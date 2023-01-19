namespace FunctionalLink.Next;

#pragma warning disable 1591
#pragma warning disable 8618

public class Value<T> 
{
    public Value(T value) =>
        Value_ = value;

    public readonly T Value_;
}

public class Other<T> 
{
    public Other(T value) =>
        Value = value;

    public readonly T Value;
}

public class None
{
    public static readonly None Value = new None();    
}

public class Error
{    
    private readonly string message;
    private readonly Exception exception;

    public Error(string message) =>
        this.message = message;

    public Error(Exception exception) =>
        this.exception = exception;

    public string Message => 
        message ?? exception.Message;

    public bool HasException() 
    {
        return exception != null;
    }

    public bool HasException(out Exception exception) 
    {
        exception = this.exception;
        return exception != null;
    }

    public static implicit operator Error(string message) =>
        new Error(message);

    public static implicit operator Error(Exception error) =>
        new Error(error);
}

