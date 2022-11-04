namespace CloudStorage;

public class Result<T>
{
    private readonly T _value;

    public Result(T value)
    {
        _value = value;
        HasValue = true;
    }

    public Result(Exception exception)
    {
        Exception = exception;
        HasValue = false;
        _value = default!;
    }

    public bool HasValue { get; }

    public T Value => HasValue ? _value : throw Exception ?? new InvalidStateException();

    public Exception? Exception { get; }
}

public sealed class InvalidStateException : Exception
{
    public InvalidStateException() : base("Result object is in invalid state")
    {
    }
}

public sealed class NotFoundException : Exception
{
    public NotFoundException(object id) : base($"Object with id {id} was not found")
    {
        Data["Id"] = id;
    }
}