using Futures.Operators;

namespace Futures.Tests.Operators;

public class DoTests
{
    [Fact]
    public void Should_Call()
    {
        var future = new Future<int>()
            .Pipe(value => value + 200)
            .Do(value => Assert.Equal(200, value));

        Assert.Equal(200, future.Next(0));
        Assert.Equal(200, future.Complete());
        Assert.Equal(200, future.Resolve());
    }
}