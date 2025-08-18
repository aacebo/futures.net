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

    public static implicit operator Action<T>(Fn<T> fn) => fn.Invoke;
    public static implicit operator Func<T, Task>(Fn<T> fn) => v => Task.Run(() => fn.Invoke(v));
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

    public Fn(Func<T, Future<TOut>> select)
    {
        _action = v => select(v).Resolve();
    }

    public TOut Invoke(T value)
    {
        return _action(value);
    }

    public static implicit operator Fn<T, TOut>(Func<T, TOut> fn) => new(fn);
    public static implicit operator Fn<T, TOut>(Func<T, Task<TOut>> fn) => new(fn);
    public static implicit operator Fn<T, TOut>(Func<T, Future<TOut>> fn) => new(fn);

    public static implicit operator Func<T, TOut>(Fn<T, TOut> fn) => fn.Invoke;
    public static implicit operator Func<T, Task<TOut>>(Fn<T, TOut> fn) => v => Task.FromResult(fn.Invoke(v));
    public static implicit operator Func<T, Future<TOut>>(Fn<T, TOut> fn) => v => Future.FromValue(fn.Invoke(v));
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

    public Fn(Func<T1, T2, Future<TOut>> stream)
    {
        _action = (a, b) => stream(a, b).Resolve();
    }

    public TOut Invoke(T1 a, T2 b)
    {
        return _action(a, b);
    }

    public static implicit operator Fn<T1, T2, TOut>(Func<T1, T2, TOut> fn) => new(fn);
    public static implicit operator Fn<T1, T2, TOut>(Func<T1, T2, Task<TOut>> fn) => new(fn);
    public static implicit operator Fn<T1, T2, TOut>(Func<T1, T2, Future<TOut>> fn) => new(fn);

    public static implicit operator Func<T1, T2, TOut>(Fn<T1, T2, TOut> fn) => fn.Invoke;
    public static implicit operator Func<T1, T2, Task<TOut>>(Fn<T1, T2, TOut> fn) => (a, b) => Task.FromResult(fn.Invoke(a, b));
    public static implicit operator Func<T1, T2, Future<TOut>>(Fn<T1, T2, TOut> fn) => (a, b) => Future.FromValue(fn.Invoke(a, b));
}

public class Fn<T1, T2, T3, TOut>
{
    private readonly Func<T1, T2, T3, TOut> _action;

    public Fn(Func<T1, T2, T3, TOut> select)
    {
        _action = select;
    }

    public Fn(Func<T1, T2, T3, Task<TOut>> select)
    {
        _action = (a, b, c) => select(a, b, c).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public Fn(Func<T1, T2, T3, Future<TOut>> stream)
    {
        _action = (a, b, c) => stream(a, b, c).Resolve();
    }

    public TOut Invoke(T1 a, T2 b, T3 c)
    {
        return _action(a, b, c);
    }

    public static implicit operator Fn<T1, T2, T3, TOut>(Func<T1, T2, T3, TOut> fn) => new(fn);
    public static implicit operator Fn<T1, T2, T3, TOut>(Func<T1, T2, T3, Task<TOut>> fn) => new(fn);
    public static implicit operator Fn<T1, T2, T3, TOut>(Func<T1, T2, T3, Future<TOut>> fn) => new(fn);

    public static implicit operator Func<T1, T2, T3, TOut>(Fn<T1, T2, T3, TOut> fn) => fn.Invoke;
    public static implicit operator Func<T1, T2, T3, Task<TOut>>(Fn<T1, T2, T3, TOut> fn) => (a, b, c) => Task.FromResult(fn.Invoke(a, b, c));
    public static implicit operator Func<T1, T2, T3, Future<TOut>>(Fn<T1, T2, T3, TOut> fn) => (a, b, c) => Future.FromValue(fn.Invoke(a, b, c));
}