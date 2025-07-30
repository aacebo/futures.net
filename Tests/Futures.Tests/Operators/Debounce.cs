using Futures.Operators;

namespace Futures.Tests.Operators;

public class DebounceTests
{
    [Fact]
    public void Should_Debounce()
    {
        var input = 0;
        var future = new Future<int>()
            .Pipe(value => value + 1)
            .Debounce(TimeSpan.FromMilliseconds(200));

        for (var i = 0; i < 10; i++)
        {
            input = future.Next(input);
            Console.WriteLine(input);
        }

        Console.WriteLine(future.Complete());
        Console.WriteLine(future.Resolve());
    }
}