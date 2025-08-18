namespace Futures;

public interface IIdentifiable
{
    Guid Id { get; }
}

public interface IStreamable<T> : IIdentifiable, IProducer<T>, IEnumerable<T>, IAsyncEnumerable<T>, IDisposable, IAsyncDisposable;