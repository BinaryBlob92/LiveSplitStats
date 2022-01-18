using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace LiveSplitStats
{
    class Program
    {
        static void Main(string[] args)
        {
            // Help
            if (args.Length == 0)
            {
                Console.WriteLine("LiveSplitStats.exe [flags] filename");
                Console.WriteLine("Available flags:");
                Console.WriteLine("  --completed");
                Console.WriteLine("      Only completed runs will be analyzed.");
                Console.WriteLine("  --from-date DATETIME");
                Console.WriteLine("      Only runs after specified date and/or time will be analyzed.");
                Console.WriteLine("  --latest-runs NUMBER");
                Console.WriteLine("      Only the latest specified number of runs will be analyzed.");
                Console.WriteLine("  --latest-segments NUMBER");
                Console.WriteLine("      Only the latest specified number of segments will be analyzed.");
                return;
            }

            // Initialize variables
            bool completedOnly = false;
            int latestRuns = 0;
            int latestSegments = 0;
            string fileName = "";
            DateTime fromDate = DateTime.MinValue;
            Run run = null;

            // Parse arguments
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--completed")
                    completedOnly = true;
                else if (args[i] == "--from-date" && i + 1 < args.Length && DateTime.TryParse(args[i + 1], out fromDate))
                    i++;
                else if (args[i] == "--latest-runs" && i + 1 < args.Length && int.TryParse(args[i + 1], out latestRuns))
                    i++;
                else if (args[i] == "--latest-segments" && i + 1 < args.Length && int.TryParse(args[i + 1], out latestSegments))
                    i++;
                else
                    fileName = args[i];
            }

            // Check filename
            if (fileName == "")
            {
                Console.WriteLine("Filename not specified..", fileName);
                return;
            }
            else if (!File.Exists(fileName))
            {
                Console.WriteLine("File '{0}' doesn't exist.", fileName);
                return;
            }

            // Read splits file
            try
            {
                using (var stream = new FileStream(fileName, FileMode.Open))
                {
                    var serializer = new XmlSerializer(typeof(Run));
                    run = (Run)serializer.Deserialize(stream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to read split file: {0}", ex.Message);
                return;
            }

            // Fetch all attempts
            var attempts = run.AttemptHistory
                .OrderByDescending(x => x.Started)
                .ToList();

            // Filter by completed only
            if (completedOnly)
            {
                attempts = attempts
                    .Where(x => x.RealTime.HasValue)
                    .ToList();
            }

            // Filter by date
            if (fromDate > DateTime.MinValue)
            {
                attempts = attempts
                    .Where(x => x.Started.HasValue && x.Started.Value >= fromDate)
                    .ToList();
            }

            // Filter by latest runs
            if (latestRuns > 0)
            {
                attempts = attempts
                    .Take(latestRuns)
                    .ToList();
            }

            Console.WriteLine("Analyzed attempts: {0}", attempts.Count);
            foreach (var segment in run.Segments)
            {
                // Join segments and attempts based on ID
                var segments = segment.SegmentHistory
                    .Join(attempts, x => x.Id, x => x.Id, (a, b) => new { Time = a, Attempt = b })
                    .OrderByDescending(x => x.Attempt.Started)
                    .ToList();

                // Filter by latest segments
                if (latestSegments > 0)
                {
                    segments = segments
                        .Take(latestSegments)
                        .ToList();
                }

                var sortedSegments = segments
                    .Where(x => x.Time.RealTime.HasValue)
                    .OrderBy(x => x.Time.RealTime)
                    .ToList();

                Console.WriteLine();
                Console.WriteLine("=== {0} ===", segment.Name);
                if (sortedSegments.Count > 0)
                {
                    var sortedTimes = sortedSegments
                        .Select(x => x.Time.RealTime.Value.TotalMilliseconds / 1000)
                        .ToList();

                    var avg = sortedTimes.Average();
                    var sum = sortedTimes.Sum(x => Math.Pow(x - avg, 2));
                    var stdDev = Math.Sqrt(sum / (sortedTimes.Count - 1));
                    var worst = sortedTimes.Reverse<double>().Take(10).Select(x => x.ToString("0.000"));
                    var best = sortedTimes.Take(10).Select(x => x.ToString("0.000"));
                    var worstAttempt = sortedSegments.Last().Attempt;
                    var bestAttempt = sortedSegments.First().Attempt;

                    Console.WriteLine("Analyzed segments: {0}", sortedTimes.Count);
                    Console.WriteLine("Average: {0:0.000}", avg);
                    Console.WriteLine("Standard deviation: {0:0.000}", stdDev);
                    Console.WriteLine("Top 10 best: {0}", string.Join(", ", best));
                    Console.WriteLine("Best attempt started at {0}", bestAttempt.Started);
                    Console.WriteLine("Top 10 worst: {0}", string.Join(", ", worst));
                    Console.WriteLine("Worst attempt started at {0}", worstAttempt.Started);

                    segment.StandardDeviation = stdDev;
                }
                else
                {
                    Console.WriteLine("NO DATA");
                    segment.StandardDeviation = double.PositiveInfinity;
                }
            }

            Console.WriteLine();
            Console.WriteLine("=== Least consistent => Most consistent ===");
            foreach (var segment in run.Segments.OrderByDescending(x => x.StandardDeviation))
                Console.WriteLine("{0} ({1:0.000})", segment.Name, segment.StandardDeviation);

#if DEBUG
            Console.ReadKey();
#endif
        }
    }
}
