namespace Futures.Operators;

public sealed class MergeMap<T, TNext>(Fn<T, Future<TNext>> selector) : ITransformer<T, TNext>
{
    public Future<T, TNext> Invoke(Future<T> src)
    {
        return new Future<T, TNext>((value, dest) =>
        {
            var @out = selector.Invoke(src.Select(value));

            foreach (var item in @out)
            {
                dest.Next(item);
            }

            dest.Complete();
        });
    }
}

public sealed class MergeMap<T, TOut, TNext>(Fn<TOut, Future<TNext>> selector) : ITransformer<T, TOut, TNext>
{
    public Future<T, TNext> Invoke(Future<T, TOut> src)
    {
        return new Future<T, TNext>((value, dest) =>
        {
            var @out = selector.Invoke(src.Select(value));

            foreach (var item in @out)
            {
                dest.Next(item);
            }

            dest.Complete();
        });
    }
}

public sealed class MergeMap<T1, T2, TOut, TNext>(Fn<TOut, Future<TNext>> selector) : ITransformer<T1, T2, TOut, TNext>
{
    public Future<T1, T2, TNext> Invoke(Future<T1, T2, TOut> src)
    {
        return new Future<T1, T2, TNext>((a, b, dest) =>
        {
            var @out = selector.Invoke(src.Select(a, b));

            foreach (var item in @out)
            {
                dest.Next(item);
            }

            dest.Complete();
        });
    }
}

public static partial class FutureExtensions
{
    public static Future<T, TNext> MergeMap<T, TNext>(this Future<T> future, Func<T, Future<TNext>> select)
    {
        return future.Pipe(new MergeMap<T, TNext>(select));
    }

    public static Future<T, TNext> MergeMap<T, TOut, TNext>(this Future<T, TOut> future, Func<TOut, Future<TNext>> select)
    {
        return future.Pipe(new MergeMap<T, TOut, TNext>(select));
    }

    public static Future<T1, T2, TNext> MergeMap<T1, T2, TOut, TNext>(this Future<T1, T2, TOut> future, Func<TOut, Future<TNext>> select)
    {
        return future.Pipe(new MergeMap<T1, T2, TOut, TNext>(select));
    }
}