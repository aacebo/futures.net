namespace Futures;

public interface IOperator<T>
{
    Future<T> Invoke(Future<T> src);
}

public interface ITransformer<T, TNext>
{
    Future<T, TNext> Invoke(Future<T> src);
}

public interface IOperator<T, TOut>
{
    Future<T, TOut> Invoke(Future<T, TOut> src);
}

public interface ITransformer<T, TOut, TNext>
{
    Future<T, TNext> Invoke(Future<T, TOut> src);
}

public interface IOperator<T1, T2, TOut>
{
    Future<T1, T2, TOut> Invoke(Future<T1, T2, TOut> src);
}

public interface ITransformer<T1, T2, TOut, TNext>
{
    Future<T1, T2, TNext> Invoke(Future<T1, T2, TOut> src);
}