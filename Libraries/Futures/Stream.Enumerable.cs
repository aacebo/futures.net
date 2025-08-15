using System.Collections;

namespace Futures;

public partial class Stream<T> : IEnumerable<T>, IAsyncEnumerable<T>
{
    public virtual IEnumerator<T> GetEnumerator()
    {
        return new Enumerator<T>(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public virtual IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new Enumerator<T>(this);
    }
}