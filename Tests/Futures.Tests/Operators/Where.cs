using Futures.Extensions;
using Futures.Operators;

namespace Futures.Tests.Operators;

public class WhereTests
{
    [Fact]
    public void Should_Filter_List()
    {
        var future = new List<int>()
            .Pipe(v => v.Select(o => (float)o))
            .Where(v => v >= 1000);

        Assert.True(future.Next([100, 200, 50000, -500]).SequenceEqual([50000]));
    }

    [Fact]
    public void Should_Filter_Dictionary()
    {
        var future = new Dictionary<int, string>()
            .Pipe()
            .Where(v => v.Key > 3)
            .Pipe(v => v.Select(o => o.Value));

        Assert.True(
            future.Next(new()
            {
                {1, "a"},
                {2, "b"},
                {3, "c"},
                {4, "d"},
                {5, "e"}
            }).SequenceEqual(["d", "e"])
        );
    }

    [Fact]
    public void Should_Filter_NonEnumerable()
    {
        var future = 0
            .Pipe()
            .Where(value => value % 2 == 0)
            .Pipe(value => value + 100);

        Assert.Equal(100, future.Next(0));
        Assert.Equal(100, future.Next(1));
        Assert.Equal(102, future.Next(2));
        Assert.Equal(102, future.Complete());
        Assert.Equal(102, future.Resolve());
    }
}