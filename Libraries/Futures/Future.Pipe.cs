namespace Futures;

public partial class Future<TIn, TOut>
{
    public IFuture<TIn, TNext> Pipe<TNext>(Func<TOut, TNext> next)
    {
        return new Future<TIn, TNext>(value =>
        {
            return next(Next(value));
        });
    }

    public IFuture<TIn, TNext> Pipe<TNext>(Func<TOut, Task<TNext>> next)
    {
        return new Future<TIn, TNext>(value =>
        {
            return next(Next(value));
        });
    }

    public IFuture<TIn, TNext> Pipe<TNext>(IFuture<TOut, TNext> next)
    {
        return new Future<TIn, TNext>(value =>
        {
            return next.Next(Next(value));
        });
    }
}