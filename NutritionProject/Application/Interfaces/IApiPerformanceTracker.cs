using Domain;

namespace Application.Interfaces
{
    public interface IApiPerformanceTracker
    {
        void Track(string uri, long durationMs);
        IReadOnlyDictionary<string, ApiPerformanceStats> GetAllPerformanceData();
    }
}
