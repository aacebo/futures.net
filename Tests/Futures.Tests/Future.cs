namespace Futures.Tests;

public class FutureTests
{
    [Fact]
    public void Should_Pipe()
    {
        var future = new Future<int>()
            .Pipe(value => value.ToString())
            .Pipe(value => value == "1");

        Assert.True(future.Next(1));
        Assert.False(future.Next(2));
        Assert.False(future.Complete());
        Assert.False(future.Resolve());
    }
}