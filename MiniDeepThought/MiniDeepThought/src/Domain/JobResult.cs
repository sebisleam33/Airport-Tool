namespace MiniDeepThought.Domain
{
    public sealed class JobResult
    {
        public string Answer { get; set; } = "";
        public string Summary { get; set; } = "";
        public long DurationMs { get; set; }
    }
}
