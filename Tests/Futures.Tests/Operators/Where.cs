using Futures.Operators;

namespace Futures.Tests.Operators;

public class WhereTests
{
    [Fact]
    public void Should_Filter()
    {
        var future = new Future<List<int>>()
            .Where(item => item % 2 == 0);

        Assert.Equal(100, future.Next(0));
        Assert.Equal(100, future.Next(1));
        Assert.Equal(102, future.Next(2));
        Assert.Equal(102, future.Complete());
        Assert.Equal(102, future.Resolve());
    }
}