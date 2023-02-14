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
    public Error(string message) =>
        (this.Message, this.Exception) = (message, null);

    public Error(Exception exception) =>
        (this.Message, this.Exception) = (exception.Message, exception);
        //this.exception = exception;

    public Error(string message, Exception exception) =>
        (this.Message, this.Exception) = (message, exception);

    public string Message { get; private set; }

    public Exception? Exception { get; private set; }

    public bool HasException() =>
        Exception != null;

    public bool HasException(out Exception exception) 
    {
        exception = this.Exception!;
        return exception != null;
    }

    public static implicit operator Error(string message) =>
        new Error(message);

    public static implicit operator Error(Exception error) =>
        new Error(error);
}

