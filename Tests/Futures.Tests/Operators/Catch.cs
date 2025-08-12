using Futures.Operators;

namespace Futures.Tests.Operators;

public class CatchTests
{
    [Fact]
    public void Should_CatchError()
    {
        var future = new Future<int>()
            .Map(value => value + 200)
            .Map(value =>
            {
                if (value % 2 != 0)
                {
                    throw new InvalidOperationException();
                }

                return value;
            })
            .Catch((_, value) => value - 200);

        Assert.Equal(200, future.Next(0));
        Assert.Equal(-199, future.Next(1));
        Assert.Equal(-199, future.Complete());
        Assert.Equal(-199, future.Resolve());
    }
}