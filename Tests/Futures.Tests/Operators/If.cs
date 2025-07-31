using Futures.Operators;

namespace Futures.Tests.Operators;

public class IfTests
{
    [Fact]
    public void Should_OnlyCallOnEvent()
    {
        var future = new Future<int>()
            .If(value => value % 2 == 0)
            .Pipe(value => value + 100);

        Assert.Equal(100, future.Next(0));
        Assert.Equal(100, future.Next(1));
        Assert.Equal(102, future.Next(2));
        Assert.Equal(102, future.Complete());
        Assert.Equal(102, future.Resolve());
    }
}