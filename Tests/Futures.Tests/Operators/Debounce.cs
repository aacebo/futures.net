using Futures.Operators;

namespace Futures.Tests.Operators;

public class DebounceTests
{
    [Fact]
    public async Task Should_Debounce()
    {
        var future = new Future<int>()
            .Pipe(value => value + 1)
            .Debounce(200);

        future.Subscribe(new()
        {
            OnNext = value => Assert.Equal(10, value)
        });

        for (var i = 0; i < 10; i++)
        {
            _ = Task.Run(() =>
            {
                Assert.Equal(i + 1, future.Next(i));
            });

            await Task.Delay(TimeSpan.FromMilliseconds(100));
        }

        await Task.Delay(TimeSpan.FromMilliseconds(200));
        Assert.Equal(10, future.Complete());
        Assert.Equal(10, future.Resolve());
    }
}