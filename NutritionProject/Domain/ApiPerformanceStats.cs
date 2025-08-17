using System.Text.Json.Serialization;

namespace Domain
{
    public class ApiPerformanceStats
    {
        [JsonPropertyName("count")]
        public int? Count { get; set; }

        [JsonPropertyName("averageMs")]
        public double? AverageMs { get; set; }

        [JsonPropertyName("minMs")]
        public long? MinMs { get; set; }

        [JsonPropertyName("maxMs")]
        public long? MaxMs { get; set; }

        [JsonPropertyName("fastCount")]
        public int? FastCount { get; set; }

        [JsonPropertyName("mediumCount")]
        public int? MediumCount { get; set; }

        [JsonPropertyName("slowCount")]
        public int   SlowCount { get; set; }

        [JsonPropertyName("durations")]
        public List<long?> Durations { get; set; } = new();
    }
}
