namespace FunctionalLink.Next;

#pragma warning disable 1591

public static class Either
{    
    public static Value<T> Value<T>(T value) =>
        new(value);

    public static Other<T> Other<T>(T value) =>
        new(value);
}
