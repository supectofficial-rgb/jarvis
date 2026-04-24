namespace Insurance.CacheService.Infra.CallerService.Models.Common;

using Insurance.CacheService.Infra.CallerService.Extensions;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class HttpService
{
    private readonly HttpClient _client;

    public HttpService(HttpClient client)
    {
        _client = client;
        _client.Timeout = Timeout.InfiniteTimeSpan;
    }

    public async Task<(TResponse? Success, TError? Error)> PostAsync<TRequest, TResponse, TError>(string url, TRequest request, string jwtToken = "", Dictionary<string, string> headers = null, int timeoutMs = 60000)
    {
        HttpResponseMessage httpResponse = null;
        try
        {
            using CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMilliseconds(timeoutMs));
            StringContent httpContent = GenerateStringContent(request);
            HttpRequestMessage httpRequest = GeneratePostHttpRequest(url, headers, httpContent, jwtToken);
            httpResponse = await _client.SendAsync(httpRequest, cts.Token);
        }
        catch (Exception)
        {
            string errorsJson = "[\"An error occurred while communicating with the service\"]";
            return (Success: default(TResponse), Error: JsonSerializer.Deserialize<TError>(errorsJson, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
        }

        string content = await httpResponse.Content.ReadAsStringAsync();
        try
        {
            httpResponse.EnsureSuccessStatusCode();
        }
        catch (Exception)
        {
            return (Success: default(TResponse), Error: JsonSerializer.Deserialize<TError>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
        }

        if (typeof(TResponse).Name == "string")
        {
            TResponse resultString = (TResponse)Convert.ChangeType(content, typeof(TResponse));
            return (Success: resultString, Error: default(TError));
        }

        if (!string.IsNullOrWhiteSpace(content))
        {
            return (Success: JsonSerializer.Deserialize<TResponse>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }), Error: default(TError));
        }

        return (Success: default(TResponse), Error: default(TError));
    }

    public async Task PostAsync<TRequest>(string url, TRequest request, string jwtToken = "", Dictionary<string, string> headers = null, int timeoutMs = 60000)
    {
        using CancellationTokenSource cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromMilliseconds(timeoutMs));
        StringContent httpContent = GenerateStringContent(request);
        HttpRequestMessage httpRequest = GeneratePostHttpRequest(url, headers, httpContent, jwtToken);
        (await _client.SendAsync(httpRequest, cts.Token)).EnsureSuccessStatusCode();
    }

    private StringContent GenerateStringContent<TRequest>(TRequest request)
    {
        string content = JsonSerializer.Serialize(request);
        return new StringContent(content, Encoding.UTF8, "application/json");
    }

    public async Task PutAsync<TRequest>(string url, TRequest request, string jwtToken = "")
    {
        string body = JsonSerializer.Serialize(request);
        StringContent httpContent = new StringContent(body, Encoding.UTF8, "application/json");
        HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, url)
        {
            Content = httpContent
        };
        if (!string.IsNullOrEmpty(jwtToken))
        {
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }

        (await _client.SendAsync(httpRequestMessage)).EnsureSuccessStatusCode();
    }

    public async Task PutAsync(string url, string jwtToken = "")
    {
        StringContent httpContent = new StringContent("", Encoding.UTF8, "application/json");
        HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, url)
        {
            Content = httpContent
        };
        if (!string.IsNullOrEmpty(jwtToken))
        {
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }

        (await _client.SendAsync(httpRequestMessage)).EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync<TRequest>(string url, TRequest request, string jwtToken = "")
    {
        HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, url + "?" + request.ToQueryString());
        if (!string.IsNullOrWhiteSpace(jwtToken))
        {
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }

        (await _client.SendAsync(httpRequestMessage)).EnsureSuccessStatusCode();
    }

    public async Task<(TResponse? Success, TError? Error)> GetAsync<TRequest, TResponse, TError>(string url, TRequest request, string jwtToken = "", Dictionary<string, string>? headers = null, int timeoutMs = 60000)
    {
        HttpResponseMessage httpResponse = null;
        try
        {
            using CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMilliseconds(timeoutMs));
            HttpRequestMessage httpRequest = GenerateGetHttpRequest(url + "?" + request.ToQueryString(), headers, jwtToken);
            httpResponse = await _client.SendAsync(httpRequest, cts.Token);
        }
        catch (Exception)
        {
            string message = "An error occurred while communicating with the service";
            string errorsJson = "[\"" + message + "\"]";
            return (Success: default(TResponse), Error: JsonSerializer.Deserialize<TError>(errorsJson, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
        }

        string content = await httpResponse.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(content))
        {
            return default((TResponse, TError));
        }

        try
        {
            httpResponse.EnsureSuccessStatusCode();
        }
        catch (Exception)
        {
            return (Success: default(TResponse), Error: JsonSerializer.Deserialize<TError>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
        }

        return (Success: JsonSerializer.Deserialize<TResponse>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }), Error: default(TError));
    }

    public async Task<(TResponse? Success, List<string>? Error)> GetAsync<TRequest, TResponse>(string url, TRequest request, string jwtToken = "", Dictionary<string, string>? headers = null, int timeoutMs = 60000)
    {
        HttpResponseMessage httpResponse = null;
        try
        {
            using CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMilliseconds(timeoutMs));
            HttpRequestMessage httpRequest = GenerateGetHttpRequest(url + "?" + request.ToQueryString(), headers, jwtToken);
            httpResponse = await _client.SendAsync(httpRequest, cts.Token);
        }
        catch (Exception)
        {
            string errorsJson = "[\"An error occurred while communicating with the service\"]";
            return (Success: default(TResponse), Error: JsonSerializer.Deserialize<List<string>>(errorsJson, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
        }

        string content = await httpResponse.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(content))
        {
            return default((TResponse, List<string>));
        }

        try
        {
            httpResponse.EnsureSuccessStatusCode();
        }
        catch (Exception)
        {
            return (Success: default(TResponse), Error: JsonSerializer.Deserialize<List<string>>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
        }

        return (Success: JsonSerializer.Deserialize<TResponse>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }), Error: null);
    }

    public async Task<TResponse?> GetAsync<TResponse>(string url, string jwtToken = "", Dictionary<string, string>? headers = null, int timeoutMs = 60000)
    {
        using CancellationTokenSource cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromMilliseconds(timeoutMs));
        HttpRequestMessage httpRequest = GenerateGetHttpRequest(url, headers, jwtToken);
        string content = await (await _client.SendAsync(httpRequest, cts.Token)).Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(content))
        {
            return default(TResponse);
        }

        return JsonSerializer.Deserialize<TResponse>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    private HttpRequestMessage GeneratePostHttpRequest(string url, Dictionary<string, string>? headers, StringContent content, string jwtToken)
    {
        HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url);
        foreach (KeyValuePair<string, string> item in headers ?? new Dictionary<string, string>())
        {
            httpRequestMessage.Headers.TryAddWithoutValidation(item.Key, item.Value);
        }

        if (!string.IsNullOrWhiteSpace(jwtToken))
        {
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }

        httpRequestMessage.Content = content;
        return httpRequestMessage;
    }

    private HttpRequestMessage GenerateGetHttpRequest(string url, Dictionary<string, string>? headers, string jwtToken)
    {
        HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url);
        foreach (KeyValuePair<string, string> item in headers ?? new Dictionary<string, string>())
        {
            httpRequestMessage.Headers.TryAddWithoutValidation(item.Key, item.Value);
        }

        if (!string.IsNullOrWhiteSpace(jwtToken))
        {
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }

        return httpRequestMessage;
    }
}