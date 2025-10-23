using MiniDeepThought.Domain;
using MiniDeepThought.Services;
using Xunit;

public class JobStoreTests
{
    [Fact]
    public async Task JobStore_SavesAndLoads_JobsRoundTrip()
    {
        var temp = Path.GetTempFileName();
        try
        {
            var store = new JobStore(temp);
            var job = Job.CreateNew("Ultimate?", "trivial");
            await store.UpsertAsync(job);

            var loaded = await store.LoadAsync();
            Assert.Contains(loaded, j => j.JobId == job.JobId);
        }
        finally
        {
            File.Delete(temp);
        }
    }
}
