namespace Futures.Operators;

public sealed class Map<T, TNext>(Fn<T, TNext> selector) : IOperator<T, TNext>
{
    public IFuture<TNext> Invoke(IFuture<T> source)
    {
        return new Future<TNext>(destination =>
        {
            source.Subscribe(new Subscriber<T>()
            {
                OnNext = value => destination.Next(selector.Resolve(value)),
                OnComplete = () => destination.Complete(),
                OnError = destination.Error,
                OnCancel = destination.Cancel
            });
        });
    }
}

public sealed class Map<T, TOut, TNext>(Fn<TOut, TNext> selector) : IOperator<T, TOut, TNext>
{
    public IFuture<T, TNext> Invoke(IFuture<T, TOut> source)
    {
        return new Future<T, TNext>(value => selector.Resolve(source.Next(value)));
    }
}

public sealed class Map<T1, T2, TOut, TNext>(Fn<TOut, TNext> selector) : IOperator<T1, T2, TOut, TNext>
{
    public IFuture<T1, T2, TNext> Invoke(IFuture<T1, T2, TOut> source)
    {
        return new Future<T1, T2, TNext>((a, b) => selector.Resolve(source.Next(a, b)));
    }
}

public static partial class FutureExtensions
{
    public static IFuture<TNext> Map<T, TNext>(this IFuture<T> future, Func<T, TNext> select)
    {
        return future.Pipe(new Map<T, TNext>(select));
    }

    public static IFuture<TNext> Map<T, TNext>(this IFuture<T> future, Func<T, Task<TNext>> select)
    {
        return future.Pipe(new Map<T, TNext>(select));
    }

    public static IFuture<TNext> Map<T, TNext>(this IFuture<T> future, Func<T, IFuture<TNext>> select)
    {
        return future.Pipe(new Map<T, TNext>(select));
    }
}

public static partial class FutureExtensions
{
    public static IFuture<T, TNext> Map<T, TOut, TNext>(this IFuture<T, TOut> future, Func<TOut, TNext> select)
    {
        return future.Pipe(new Map<T, TOut, TNext>(select));
    }

    public static IFuture<T, TNext> Map<T, TOut, TNext>(this IFuture<T, TOut> future, Func<TOut, Task<TNext>> select)
    {
        return future.Pipe(new Map<T, TOut, TNext>(select));
    }

    public static IFuture<T, TNext> Map<T, TOut, TNext>(this IFuture<T, TOut> future, Func<TOut, IFuture<TNext>> select)
    {
        return future.Pipe(new Map<T, TOut, TNext>(select));
    }
}

public static partial class FutureExtensions
{
    public static IFuture<T1, T2, TNext> Map<T1, T2, TOut, TNext>(this IFuture<T1, T2, TOut> future, Func<TOut, TNext> select)
    {
        return future.Pipe(new Map<T1, T2, TOut, TNext>(select));
    }

    public static IFuture<T1, T2, TNext> Map<T1, T2, TOut, TNext>(this IFuture<T1, T2, TOut> future, Func<TOut, Task<TNext>> select)
    {
        return future.Pipe(new Map<T1, T2, TOut, TNext>(select));
    }

    public static IFuture<T1, T2, TNext> Map<T1, T2, TOut, TNext>(this IFuture<T1, T2, TOut> future, Func<TOut, IFuture<TNext>> select)
    {
        return future.Pipe(new Map<T1, T2, TOut, TNext>(select));
    }
}