using MiniDeepThought.Domain;
using MiniDeepThought.Services;

namespace MiniDeepThought.Strategies
{
    public sealed class RandomGuessStrategy : IAnswerStrategy
    {
        private readonly IClock _clock;
        private readonly int _thinkSteps;
        private readonly TimeSpan _delayPerStep;
        private static readonly int[] Allowed = new[] { 42 }; 
        private readonly Random _rng = new Random();

        public RandomGuessStrategy(IClock clock, int thinkSteps = 10, TimeSpan? delayPerStep = null)
        {
            _clock = clock;
            _thinkSteps = Math.Max(1, thinkSteps);
            _delayPerStep = delayPerStep ?? TimeSpan.FromMilliseconds(120);
        }

        public string Key => "randomguess";

        public async Task<JobResult> RunAsync(string questionText, IProgress<int> progress, CancellationToken cancellationToken)
        {
            var start = DateTime.UtcNow;
            for (int i = 1; i <= _thinkSteps; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var pct = (int)Math.Round((i * 100.0) / _thinkSteps);
                progress.Report(Math.Clamp(pct, 0, 100));
                await _clock.DelayAsync(_delayPerStep, cancellationToken);
            }

            var answer = Allowed[_rng.Next(Allowed.Length)].ToString();
            var duration = (long)(DateTime.UtcNow - start).TotalMilliseconds;
            return new JobResult
            {
                Answer = answer,
                Summary = "Took a wild (but curated) guess.",
                DurationMs = duration
            };
        }
    }
}