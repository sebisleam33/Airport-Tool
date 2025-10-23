using MiniDeepThought.Domain;
using MiniDeepThought.Strategies;

namespace MiniDeepThought.Services
{
    public interface IClock
    {
        Task DelayAsync(TimeSpan delay, CancellationToken cancellationToken);
    }

    public sealed class SystemClock : IClock
    {
        public Task DelayAsync(TimeSpan delay, CancellationToken cancellationToken) => Task.Delay(delay, cancellationToken);
    }

    public sealed class JobRunner
    {
        private readonly JobStore _store;
        private readonly IClock _clock;

        public JobRunner(JobStore store, IClock clock)
        {
            _store = store;
            _clock = clock;
        }

        public async Task RunAsync(Job job, IAnswerStrategy strategy, IProgress<int> progress, CancellationToken cancellationToken)
        {
            if (job.IsTerminal)
                return;

            job.Status = JobStatus.Running;
            job.StartedUtc = DateTime.UtcNow;
            job.Progress = 0;
            await _store.UpsertAsync(job);

            try
            {
                var result = await strategy.RunAsync(job.QuestionText, progress, cancellationToken);
                job.Status = JobStatus.Completed;
                job.Result = result;
                job.Progress = 100;
                job.CompletedUtc = DateTime.UtcNow;
            }
            catch (OperationCanceledException)
            {
                job.Status = JobStatus.Canceled;
                job.CompletedUtc = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                job.Status = JobStatus.Failed;
                job.Result = new JobResult { Answer = string.Empty, Summary = $"Failed: {ex.Message}", DurationMs = 0 };
                job.CompletedUtc = DateTime.UtcNow;
            }
            finally
            {
                await _store.UpsertAsync(job);
            }
        }
    }
}
