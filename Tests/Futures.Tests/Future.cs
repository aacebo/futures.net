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

    [Fact]
    public async Task Should_Async_Enumerate()
    {
        var future = new Future<int>();
        var task = Task.Run(async () =>
        {
            var i = -1;

            await foreach (var update in future)
            {
                i++;
                Assert.Equal(i, update);
            }
        });

        await Task.Delay(100);

        for (var i = 0; i < 10; i++)
        {
            future.Next(i);
        }

        future.Complete();
        await task;
    }
}