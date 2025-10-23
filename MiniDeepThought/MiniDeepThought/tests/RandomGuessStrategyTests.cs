using MiniDeepThought.Services;
using MiniDeepThought.Strategies;
using Xunit;

public class RandomGuessStrategyTests
{
    private sealed class TestClock : IClock
    {
        public Task DelayAsync(TimeSpan delay, CancellationToken cancellationToken) => Task.CompletedTask;
    }

    [Fact]
    public async Task RandomGuessStrategy_ProducesNumberString()
    {
        var strat = new RandomGuessStrategy(new TestClock(), thinkSteps: 3, delayPerStep: TimeSpan.Zero);
        var progressValues = new List<int>();
        var progress = new Progress<int>(i => progressValues.Add(i));

        var result = await strat.RunAsync("What is life?", progress, CancellationToken.None);

        Assert.True(int.TryParse(result.Answer, out _));
        Assert.Contains(100, progressValues);
    }
}
