namespace Futures;

public interface IOperator<T, TOut, TNext>
{
    Future<T, TNext> Invoke(Future<T, TOut> source);
}