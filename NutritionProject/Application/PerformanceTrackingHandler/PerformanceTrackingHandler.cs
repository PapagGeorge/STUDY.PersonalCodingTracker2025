using Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.PerformanceTrackingHandler
{
    public class PerformanceTrackingHandler : DelegatingHandler
    {
        private readonly IApiPerformanceTracker _tracker;
        private readonly ILogger<PerformanceTrackingHandler> _logger;

        public PerformanceTrackingHandler(IApiPerformanceTracker tracker, ILogger<PerformanceTrackingHandler> logger)
        {
            _tracker = tracker;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            var response = await base.SendAsync(request, cancellationToken);

            stopwatch.Stop();

            var elapsedMs = stopwatch.ElapsedMilliseconds;

            _tracker.Track(request.RequestUri?.ToString() ?? "Unknown", elapsedMs);

            _logger.LogInformation("API call to {Uri} took {ElapsedMilliseconds} ms", request.RequestUri, elapsedMs);

            return response;
        }
    }
}
