using System.Text.Json;
using System.Text.Json.Serialization;
using MiniDeepThought.Domain;

namespace MiniDeepThought.Services
{
    public sealed class JobStore
    {
        private readonly string _filePath;
        private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public JobStore(string filePath) => _filePath = filePath;

        public async Task<List<Job>> LoadAsync()
        {
            if (!File.Exists(_filePath))
                return new List<Job>();

            await using var fs = File.OpenRead(_filePath);
            var jobs = await JsonSerializer.DeserializeAsync<List<Job>>(fs, _jsonOptions) ?? new List<Job>();
            return jobs.OrderBy(j => j.CreatedUtc).ToList();
        }

        public async Task UpsertAsync(Job job)
        {
            var jobs = await LoadAsync();
            var idx = jobs.FindIndex(j => j.JobId == job.JobId);
            if (idx >= 0) jobs[idx] = job; else jobs.Add(job);

            await using var fs = File.Create(_filePath);
            await JsonSerializer.SerializeAsync(fs, jobs, _jsonOptions);
        }
    }
}