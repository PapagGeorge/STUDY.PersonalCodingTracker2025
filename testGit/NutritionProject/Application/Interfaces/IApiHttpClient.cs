namespace Application.Interfaces
{
    public interface IApiHttpClient
    {
        Task<TResponse> PostAsync<TRequest, TResponse>(string uri, TRequest data, Dictionary<string, string>? headers = null);
    }
}