using System.Globalization;
using System.Text.Json;
using MiniDeepThought.Domain;

namespace MiniDeepThought.Util
{
    public static class ConsoleHelpers
    {
        public static void PrintTitle(string title)
        {
            Console.WriteLine(new string('=', Math.Max(8, title.Length + 6)));
            Console.WriteLine($"== {title} ==");
            Console.WriteLine(new string('=', Math.Max(8, title.Length + 6)));
        }

        public static string? PromptUntilValid(string prompt, Func<string?, bool> validator, string errorMessage)
        {
            while (true)
            {
                Console.Write(prompt);
                var input = Console.ReadLine();
                if (validator(input))
                    return input;
                WriteWarning(errorMessage);
            }
        }

        public static void WriteWarning(string message)
        {
            var prev = Console.ForegroundColor; Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ForegroundColor = prev;
        }

        public static void WriteSuccess(string message)
        {
            var prev = Console.ForegroundColor; Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ForegroundColor = prev;
        }

        public static void WriteLineDim(string message)
        {
            var prev = Console.ForegroundColor; Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(message);
            Console.ForegroundColor = prev;
        }

        public static void WriteProgress(int percent)
        {
            Console.Write("\rProgress: {0,3}%   ", Math.Clamp(percent, 0, 100));
            if (percent >= 100) Console.WriteLine();
        }

        public static void PrintJobsTable(IEnumerable<Job> jobs)
        {
            Console.WriteLine("JobId                                 | Status    | Algorithm   | CreatedUtc (UTC)       | Progress");
            Console.WriteLine(new string('-', 100));
            foreach (var j in jobs)
            {
                Console.WriteLine($"{j.JobId} | {j.Status,-9} | {j.AlgorithmKey,-11} | {j.CreatedUtc:yyyy-MM-dd HH:mm:ss} | {j.Progress,3}%");
            }
        }

        public static void WriteJson(object payload)
        {
            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine(json);
        }
    }
}
