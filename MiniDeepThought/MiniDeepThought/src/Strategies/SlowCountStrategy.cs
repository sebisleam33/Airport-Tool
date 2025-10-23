using MiniDeepThought.Domain;
using MiniDeepThought.Services;

namespace MiniDeepThought.Strategies
{
    public sealed class SlowCountStrategy : IAnswerStrategy
    {
        private readonly IClock _clock;
        private readonly int _stepCount;
        private readonly TimeSpan _delayPerStep;

        public SlowCountStrategy(IClock clock, int stepCount = 20, TimeSpan? delayPerStep = null)
        {
            _clock = clock;
            _stepCount = Math.Max(1, stepCount);
            _delayPerStep = delayPerStep ?? TimeSpan.FromMilliseconds(150);
        }

        public string Key => "slowcount";

        public async Task<JobResult> RunAsync(string questionText, IProgress<int> progress, CancellationToken cancellationToken)
        {
            var start = DateTime.UtcNow;
            for (int i = 1; i <= _stepCount; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var pct = (int)Math.Round((i * 100.0) / _stepCount);
                progress.Report(Math.Clamp(pct, 0, 100));
                await _clock.DelayAsync(_delayPerStep, cancellationToken);
            }

            var duration = (long)(DateTime.UtcNow - start).TotalMilliseconds;
            return new JobResult
            {
                Answer = "42",
                Summary = $"Counted slowly in {_stepCount} steps.",
                DurationMs = duration
            };
        }
    }
}
