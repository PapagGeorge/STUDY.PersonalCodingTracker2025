using Application.Interfaces;
using Domain;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Application.PerformanceTrackingHandler
{
    public class ApiPerformanceTracker : IApiPerformanceTracker
    {
        private readonly ILogger<ApiPerformanceTracker> _logger;
        private readonly ConcurrentDictionary<string, ApiPerformanceStats> _stats = new();

        public ApiPerformanceTracker(ILogger<ApiPerformanceTracker> logger)
        {
            _logger = logger;
        }

        public void Track(string uri, long durationMs)
        {
            string category = durationMs switch
            {
                <= 300 => "FAST",
                <= 1000 => "MEDIUM",
                _ => "SLOW"
            };

            _logger.LogInformation("Performance category for {Uri}: {Category} ({Duration} ms)", uri, category, durationMs);

            var stat = _stats.GetOrAdd(uri, _ => new ApiPerformanceStats());

            lock (stat)
            {
                stat.Durations.Add(durationMs);
                stat.Count++;
                stat.AverageMs = stat.Durations.Average();
                stat.MinMs = stat.Durations.Min();
                stat.MaxMs = stat.Durations.Max();

                switch (category)
                {
                    case "FAST": stat.FastCount++; break;
                    case "MEDIUM": stat.MediumCount++; break;
                    case "SLOW": stat.SlowCount++; break;
                }
            }
        }

        public IReadOnlyDictionary<string, ApiPerformanceStats> GetAllPerformanceData()
        {
            var result = new Dictionary<string, ApiPerformanceStats>();

            foreach (var entry in _stats)
            {
                var uri = entry.Key;
                List<long> durations;

                lock (entry.Value)
                {
                    durations = entry.Value.Durations.Where(d => d.HasValue).Select(d => d.Value).ToList();
                }

                if (durations.Count == 0)
                {
                    result[uri] = new ApiPerformanceStats();
                    continue;
                }

                var count = durations.Count;
                var average = durations.Average();
                var min = durations.Min();
                var max = durations.Max();
                var fastCount = durations.Count(d => d <= 300);
                var mediumCount = durations.Count(d => d > 300 && d <= 1000);
                var slowCount = durations.Count(d => d > 1000);

                result[uri] = new ApiPerformanceStats
                {
                    Count = count,
                    AverageMs = (int)average,
                    MinMs = min,
                    MaxMs = max,
                    FastCount = fastCount,
                    MediumCount = mediumCount,
                    SlowCount = slowCount,
                    Durations = durations.Select(d => (long?)d).ToList()
                };
            }

            return result;
        }

    }
}
