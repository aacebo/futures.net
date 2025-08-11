
namespace Futures;

public partial class Stream<T, TOut> : IStream<T, TOut>
{
    private readonly IConsumer<T> _consumer;
    private readonly IProducer<TOut> _producer;

    public Stream(IConsumer<T> consumer, IProducer<TOut> producer)
    {
        _consumer = consumer;
        _producer = producer;
    }

    public TOut Next(T value)
    {
        throw new NotImplementedException();
    }
}

public partial class Stream<T, TOut> : IConsumer<T>
{
    public void OnCancel()
    {
        _consumer.OnCancel();
    }

    public void OnComplete()
    {
        _consumer.OnComplete();
    }

    public void OnError(Exception ex)
    {
        _consumer.OnError(ex);
    }

    public void OnNext(T value)
    {
        _consumer.OnNext(value);
    }
}

public partial class Stream<T, TOut> : IProducer<TOut>
{
    public void Cancel()
    {
        _producer.Cancel();
    }

    public void Complete()
    {
        _producer.Complete();
    }

    public void Error(Exception ex)
    {
        _producer.Error(ex);
    }

    public void Next(TOut value)
    {
        _producer.Next(value);
    }
}