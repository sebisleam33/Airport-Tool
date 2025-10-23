using MiniDeepThought.Domain;

namespace MiniDeepThought.Strategies
{
    public interface IAnswerStrategy
    {
        string Key { get; }
        Task<JobResult> RunAsync(string questionText, IProgress<int> progress, CancellationToken cancellationToken);
    }
}