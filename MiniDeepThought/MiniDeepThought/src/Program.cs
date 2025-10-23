using MiniDeepThought.Domain;
using MiniDeepThought.Services;
using MiniDeepThought.Strategies;
using MiniDeepThought.Util;
using System.Text.Json;

public class Program
{
    private static readonly string DataFile = "deepthought-jobs.json";

    public static async Task Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var jobStore = new JobStore(DataFile);
        var jobs = await jobStore.LoadAsync();
        var clock = new SystemClock();
        var runner = new JobRunner(jobStore, clock);

        IAnswerStrategy ResolveStrategy(string key) => key.ToLowerInvariant() switch
        {
            "trivial" => new TrivialStrategy(clock),
            "slowcount" => new SlowCountStrategy(clock, stepCount: 20, delayPerStep: TimeSpan.FromMilliseconds(120)),
            "randomguess" => new RandomGuessStrategy(clock, thinkSteps: 12, delayPerStep: TimeSpan.FromMilliseconds(100)),
            _ => throw new ArgumentException("Unknown strategy")
        };

        ConsoleHelpers.PrintTitle("Mini Deep Thought");
        ConsoleHelpers.WriteLineDim("Jobs will persist to '" + DataFile + "'.");

        Job? runningJob = null;
        CancellationTokenSource? cts = null;

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("1) Submit Question");
            Console.WriteLine("2) List Jobs");
            Console.WriteLine("3) View Result by JobId");
            Console.WriteLine("4) Cancel Running Job");
            Console.WriteLine("5) Exit");
            Console.Write("Select: ");
            var choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    {
                        var question = ConsoleHelpers.PromptUntilValid(
                            "Enter your Ultimate Question (1-200): ",
                            text => !string.IsNullOrWhiteSpace(text) && text.Trim().Length is >= 1 and <= 200,
                            "Please enter between 1 and 200 characters.");

                        var algorithm = ConsoleHelpers.PromptUntilValid(
                            "Algorithm [Trivial|SlowCount|RandomGuess]: ",
                            text => text is not null && new[] { "trivial", "slowcount", "randomguess" }
                                .Contains(text.Trim().ToLowerInvariant()),
                            "Unknown algorithm. Please type Trivial, SlowCount, or RandomGuess.");

                        var job = Job.CreateNew(question!.Trim(), CultureInfoInvariantLower(algorithm!));
                        await jobStore.UpsertAsync(job);
                        ConsoleHelpers.WriteSuccess($"Job queued: {job.JobId}");

                        if (runningJob is not null)
                        {
                            ConsoleHelpers.WriteLineDim("Another job is running; please wait or cancel it first.");
                            break;
                        }

                        runningJob = job;
                        cts = new CancellationTokenSource();

                        var progress = new Progress<int>(p =>
                        {
                            job.Progress = p;
                            ConsoleHelpers.WriteProgress(p);
                        });

                        ConsoleHelpers.WriteLineDim("Tip: Press 'C' to cancel while it runs.");

                        var strategy = ResolveStrategy(job.AlgorithmKey);
                        var runTask = runner.RunAsync(job, strategy, progress, cts.Token);

                        while (!runTask.IsCompleted)
                        {
                            if (Console.KeyAvailable)
                            {
                                var key = Console.ReadKey(intercept: true);
                                if (char.ToUpperInvariant(key.KeyChar) == 'C')
                                {
                                    cts.Cancel();
                                    ConsoleHelpers.WriteWarning("Cancellation requested...");
                                }
                            }
                            await clock.DelayAsync(TimeSpan.FromMilliseconds(50), CancellationToken.None);
                        }

                        try { await runTask; }
                        catch (OperationCanceledException) { }

                        runningJob = null;
                        cts.Dispose();
                        cts = null;
                        break;
                    }
                case "2":
                    ConsoleHelpers.PrintJobsTable(await jobStore.LoadAsync());
                    break;
                case "3":
                    {
                        Console.Write("JobId: ");
                        var input = Console.ReadLine()?.Trim();
                        if (!Guid.TryParse(input, out var id))
                        {
                            ConsoleHelpers.WriteWarning("That's not a valid GUID.");
                            break;
                        }
                        var job = jobs.FirstOrDefault(j => j.JobId == id) ?? (await jobStore.LoadAsync()).FirstOrDefault(j => j.JobId == id);
                        if (job is null)
                        {
                            ConsoleHelpers.WriteWarning("Job not found.");
                            break;
                        }
                        if (job.Status == JobStatus.Completed && job.Result is not null)
                        {
                            var payload = new
                            {
                                jobId = job.JobId,
                                answer = job.Result.Answer,
                                summary = job.Result.Summary,
                                durationMs = job.Result.DurationMs
                            };
                            ConsoleHelpers.WriteJson(payload);
                        }
                        else
                        {
                            ConsoleHelpers.WriteLineDim("This job is not completed yet.");
                        }
                        break;
                    }
                case "4":
                    if (cts is not null)
                    {
                        cts.Cancel();
                        ConsoleHelpers.WriteWarning("Cancellation requested.");
                    }
                    else
                    {
                        ConsoleHelpers.WriteLineDim("No job is currently running.");
                    }
                    break;
                case "5":
                    ConsoleHelpers.WriteLineDim("Goodbye!");
                    return;
                default:
                    ConsoleHelpers.WriteWarning("Invalid selection. Please choose 1-5.");
                    break;
            }

            jobs = await jobStore.LoadAsync();
        }
    }

    private static string CultureInfoInvariantLower(string s) => s.Trim().ToLowerInvariant();
}
