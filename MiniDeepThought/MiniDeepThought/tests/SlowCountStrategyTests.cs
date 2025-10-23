using MiniDeepThought.Services;
using MiniDeepThought.Strategies;
using Xunit;

public class SlowCountStrategyTests
{
    private sealed class TestClock : IClock
    {
        public Task DelayAsync(TimeSpan delay, CancellationToken cancellationToken) => Task.CompletedTask;
    }

    [Fact]
    public async Task SlowCountStrategy_AdvancesProgress_To100()
    {
        var strat = new SlowCountStrategy(new TestClock(), stepCount: 5, delayPerStep: TimeSpan.Zero);
        var progressValues = new List<int>();
        var progress = new Progress<int>(i => progressValues.Add(i));

        var result = await strat.RunAsync("How?", progress, CancellationToken.None);

        Assert.Equal("42", result.Answer);
        Assert.Contains(100, progressValues);
    }

    [Fact]
    public async Task SlowCountStrategy_HonorsCancellation_BeforeCompletion()
    {
        var cts = new CancellationTokenSource();
        var strat = new SlowCountStrategy(new TestClock(), stepCount: 100, delayPerStep: TimeSpan.Zero);
        var progressValues = new List<int>();
        var progress = new Progress<int>(i =>
        {
            progressValues.Add(i);
            if (i >= 20) cts.Cancel();
        });

        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            await strat.RunAsync("Cancel?", progress, cts.Token);
        });

        Assert.Contains(progressValues, p => p < 100);
    }
}
