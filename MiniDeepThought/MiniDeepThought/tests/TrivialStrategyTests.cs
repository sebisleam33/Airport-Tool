using MiniDeepThought.Domain;
using MiniDeepThought.Services;
using MiniDeepThought.Strategies;
using Xunit;

public class TrivialStrategyTests
{
    private sealed class TestClock : IClock
    {
        public Task DelayAsync(TimeSpan delay, CancellationToken cancellationToken) => Task.CompletedTask;
    }

    [Fact]
    public async Task TrivialStrategy_Returns42_AndReportsProgress()
    {
        var strat = new TrivialStrategy(new TestClock());
        var progressValues = new List<int>();
        var progress = new Progress<int>(i => progressValues.Add(i));

        var result = await strat.RunAsync("Why?", progress, CancellationToken.None);

        Assert.Equal("42", result.Answer);
        Assert.Contains(100, progressValues);
    }
}
