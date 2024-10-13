using RestSharp;

namespace JupiterPrime.Infrastructure.Clients;

public class ApiClient
{
    private readonly RestClient _client;

    public ApiClient(string baseUrl)
    {
        _client = new RestClient(baseUrl);
    }

    public async Task<RestResponse> ExecuteAsync(RestRequest request)
    {
        return await _client.ExecuteAsync(request);
    }
}

