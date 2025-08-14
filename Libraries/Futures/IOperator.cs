namespace Futures;

public interface IOperator<T, TNext>
{
    IFuture<TNext> Invoke(IFuture<T> source);
}

public interface IOperator<T, TOut, TNext>
{
    IFuture<T, TNext> Invoke(IFuture<T, TOut> source);
}

public interface IOperator<T1, T2, TOut, TNext>
{
    IFuture<T1, T2, TNext> Invoke(IFuture<T1, T2, TOut> source);
}