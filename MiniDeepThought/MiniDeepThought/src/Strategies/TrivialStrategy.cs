using MiniDeepThought.Domain;
using MiniDeepThought.Services;

namespace MiniDeepThought.Strategies
{
    public sealed class TrivialStrategy : IAnswerStrategy
    {
        private readonly IClock _clock;
        public TrivialStrategy(IClock clock) => _clock = clock;
        public string Key => "trivial";

        public async Task<JobResult> RunAsync(string questionText, IProgress<int> progress, CancellationToken cancellationToken)
        {
            progress.Report(0);
            await _clock.DelayAsync(TimeSpan.FromMilliseconds(50), cancellationToken);
            progress.Report(100);
            return new JobResult
            {
                Answer = "42",
                Summary = "Trivial strategy: the answer is always 42.",
                DurationMs = 50
            };
        }
    }
}
