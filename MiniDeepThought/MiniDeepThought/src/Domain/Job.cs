using System.Text.Json.Serialization;

namespace MiniDeepThought.Domain
{
    public enum JobStatus { Pending, Running, Completed, Canceled, Failed }

    public sealed class Job
    {
        public Guid JobId { get; set; }
        public string QuestionText { get; set; } = "";
        public string AlgorithmKey { get; set; } = "trivial";
        public JobStatus Status { get; set; }
        public int Progress { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime? StartedUtc { get; set; }
        public DateTime? CompletedUtc { get; set; }
        public JobResult? Result { get; set; }

        [JsonIgnore]
        public bool IsTerminal => Status is JobStatus.Completed or JobStatus.Canceled or JobStatus.Failed;

        public static Job CreateNew(string questionText, string algorithmKey) => new()
        {
            JobId = Guid.NewGuid(),
            QuestionText = questionText,
            AlgorithmKey = algorithmKey,
            Status = JobStatus.Pending,
            Progress = 0,
            CreatedUtc = DateTime.UtcNow
        };
    }
}