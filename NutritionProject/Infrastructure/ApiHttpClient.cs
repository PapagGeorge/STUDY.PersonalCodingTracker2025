using Application.Interfaces;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Infrastructure
{
    public class ApiHttpClient : IApiHttpClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ApiHttpClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(string uri, TRequest data, Dictionary<string, string>? headers = null)
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

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(responseContent)!;
        }
    }
}
