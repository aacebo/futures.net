namespace Futures;

public interface IOperator<T>
{
    IFuture<T> Invoke(IFuture<T> source);
}

public interface IOperator<T, TOut>
{
    IFuture<TOut> Invoke(IFuture<T> source);
}
