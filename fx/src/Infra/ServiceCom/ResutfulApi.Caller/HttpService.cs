using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class HttpService
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public HttpService(HttpClient client)
    {
        _client = client;
        _client.Timeout = Timeout.InfiniteTimeSpan;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    // ------------------------
    //  POST
    // ------------------------
    public async Task<(TResponse? Success, TError? Error)> PostAsync<TRequest, TResponse, TError>(
        string url,
        TRequest request,
        string jwtToken = "",
        Dictionary<string, string>? headers = null,
        int timeoutMs = 60000)
    {
        try
        {
            using var cts = new CancellationTokenSource(timeoutMs);

            HttpRequestMessage msg = new(HttpMethod.Post, url)
            {
                Content = JsonContent(request)
            };

            ApplyHeaders(msg, jwtToken, headers);

            HttpResponseMessage httpResponse = await _client.SendAsync(msg, cts.Token);

            return await ParseResponse<TResponse, TError>(httpResponse);
        }
        catch
        {
            return (default, CreateDefaultError<TError>());
        }
    }

    // ------------------------
    //  GET
    // ------------------------
    public async Task<(TResponse? Success, TError? Error)> GetAsync<TResponse, TError>(
        string url,
        string jwtToken = "",
        Dictionary<string, string>? headers = null,
        int timeoutMs = 60000)
    {
        try
        {
            using var cts = new CancellationTokenSource(timeoutMs);

            HttpRequestMessage msg = new(HttpMethod.Get, url);
            ApplyHeaders(msg, jwtToken, headers);

            HttpResponseMessage httpResponse = await _client.SendAsync(msg, cts.Token);

            return await ParseResponse<TResponse, TError>(httpResponse);
        }
        catch
        {
            return (default, CreateDefaultError<TError>());
        }
    }

    // ================================================================
    // PRIVATE HELPERS
    // ================================================================

    private StringContent JsonContent<T>(T data)
        => new(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

    private void ApplyHeaders(HttpRequestMessage msg, string jwtToken, Dictionary<string, string>? headers)
    {
        if (!string.IsNullOrWhiteSpace(jwtToken))
            msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        if (headers != null)
            foreach (var h in headers)
                msg.Headers.TryAddWithoutValidation(h.Key, h.Value);
    }

    private async Task<(TResponse?, TError?)> ParseResponse<TResponse, TError>(HttpResponseMessage httpResponse)
    {
        string content = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
        {
            TError? err = JsonSerializer.Deserialize<TError>(content, _jsonOptions);
            return (default, err);
        }

        if (typeof(TResponse) == typeof(string))
            return ((TResponse?)(object?)content, default);

        TResponse? success = JsonSerializer.Deserialize<TResponse>(content, _jsonOptions);
        return (success, default);
    }

    private TError CreateDefaultError<TError>()
    {
        string json = "[\"An error occurred while communicating with the service\"]";
        return JsonSerializer.Deserialize<TError>(json, _jsonOptions)!;
    }
}
