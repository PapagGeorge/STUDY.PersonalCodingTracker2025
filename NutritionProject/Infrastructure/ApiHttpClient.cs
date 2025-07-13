using Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Infrastructure
{
    public class ApiHttpClient : IApiHttpClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ApiHttpClient> _logger;

        public ApiHttpClient(IHttpClientFactory httpClientFactory, ILogger<ApiHttpClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(    string uri, TRequest data, Dictionary<string, string>? headers = null)
        {
            _logger.LogInformation("Sending POST request to {Uri}", uri);
            try
            {
                var client = _httpClientFactory.CreateClient("ExternalApiClient");

                var request = new HttpRequestMessage(HttpMethod.Post, uri)
                {
                    Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json")
                };

                if (headers != null)
                {
                    foreach (var kvp in headers)
                        request.Headers.TryAddWithoutValidation(kvp.Key, kvp.Value);
                }

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("POST request to {Uri} succeeded with status code {StatusCode}", uri, response.StatusCode);
                return JsonSerializer.Deserialize<TResponse>(content)!;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed for {Uri}", uri);
                throw new ExternalApiException("Error communicating with external API.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in ApiHttpClient.");
                throw;
            }
        }
    }
}
