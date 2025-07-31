namespace Futures;

public class CancelledException : Exception
{
    public CancelledException() : base()
    {

    }

    public CancelledException(string message) : base(message)
    {

    }
}