namespace Futures;

public class Fn<T>
{
    private readonly Action<T> _action;

    public Fn(Action<T> select)
    {
        _action = select;
    }

    public Fn(Func<T, Task> select)
    {
        _action = v => select(v).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public void Invoke(T value)
    {
        _action(value);
    }

    public static implicit operator Fn<T>(Action<T> fn) => new(fn);
    public static implicit operator Fn<T>(Func<T, Task> fn) => new(fn);
}

/// <summary>
/// Encapsulates a method that has one parameter and returns
/// a value of the type specified by the TOut parameter.
/// </summary>
public class Fn<T, TOut>
{
    private readonly Func<T, TOut> _action;

    public Fn(Func<T, TOut> select)
    {
        _action = select;
    }

    public Fn(Func<T, Task<TOut>> select)
    {
        _action = v => select(v).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public Fn(Func<T, IFuture<TOut>> select)
    {
        _action = v => select(v).Resolve();
    }

    public TOut Invoke(T value)
    {
        return _action(value);
    }

    public static implicit operator Fn<T, TOut>(Func<T, TOut> fn) => new(fn);
    public static implicit operator Fn<T, TOut>(Func<T, Task<TOut>> fn) => new(fn);
    public static implicit operator Fn<T, TOut>(Func<T, IFuture<TOut>> fn) => new(fn);
}

public class Fn<T1, T2, TOut>
{
    private readonly Func<T1, T2, TOut> _action;

    public Fn(Func<T1, T2, TOut> select)
    {
        _action = select;
    }

    public Fn(Func<T1, T2, Task<TOut>> select)
    {
        _action = (a, b) => select(a, b).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public Fn(Func<T1, T2, IFuture<TOut>> select)
    {
        _action = (a, b) => select(a, b).Resolve();
    }

    public TOut Invoke(T1 a, T2 b)
    {
        return _action(a, b);
    }

    public static implicit operator Fn<T1, T2, TOut>(Func<T1, T2, TOut> fn) => new(fn);
    public static implicit operator Fn<T1, T2, TOut>(Func<T1, T2, Task<TOut>> fn) => new(fn);
    public static implicit operator Fn<T1, T2, TOut>(Func<T1, T2, IFuture<TOut>> fn) => new(fn);
}