using Futures.Operators;

namespace Futures.Tests.Operators;

public class MergeTests
{
    [Fact]
    public void Should_Merge()
    {
        var a = new Future<string>().ToProducer();
        var b = new Future<int>().ToProducer();
        var merged = a.Merge(b);
        var input = new Future<int>()
            .ToProducer()
            .Do(value => merged.Next((value.ToString(), value)));

        for (var i = 0; i < 10; i++)
        {
            input.Next(i);
        }

        Assert.Equal(("9", 9), merged.Complete());
        Assert.Equal("9", a.Complete());
        Assert.Equal(9, b.Complete());
    }
}