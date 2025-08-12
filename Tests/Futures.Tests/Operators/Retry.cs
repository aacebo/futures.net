using Futures.Operators;

namespace Futures.Tests.Operators;

public class RetryTests
{
    [Fact]
    public void Should_Retry()
    {
        var i = 0;
        var future = new Future<int>()
            .Map(value =>
            {
                i++;

                if (i % 2 != 0)
                {
                    throw new InvalidOperationException();
                }

                return value + i;
            })
            .Retry();

        Assert.Equal(2, future.Next(0));
        Assert.Equal(4, future.Next(0));
        Assert.Equal(4, future.Complete());
        Assert.Equal(4, future.Resolve());
    }
}