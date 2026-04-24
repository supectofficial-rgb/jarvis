using Insurance.WebApp.Panel.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Insurance.WebApp.Panel.Services
{
    // Wrapper classes for API responses
    internal class CommandResultWrapper<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public List<string>? ErrorMessages { get; set; }
        public Dictionary<string, string[]>? ValidationErrors { get; set; }
    }

    internal class QueryResultWrapper<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ErrorCode { get; set; }
        public IEnumerable<string>? ValidationErrors { get; set; }
    }

    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ApiService> _logger;

        public ApiService(HttpClient httpClient, IConfiguration configuration, ILogger<ApiService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;

            var apiGatewayUrl = _configuration["ApiGateway:BaseUrl"] ?? "https://localhost:7228";
            _httpClient.BaseAddress = new Uri(apiGatewayUrl);
        }

        public async Task<ApiResponse<LoginResponse>> LoginAsync(string userName, string password)
        {
            try
            {
                var loginRequest = new
                {
                    UserName = userName,
                    Password = password
                };

                var json = JsonSerializer.Serialize(loginRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/UserService/Auth/login/by-credential", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var commandResult = JsonSerializer.Deserialize<CommandResultWrapper<LoginResponse>>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (commandResult != null && commandResult.IsSuccess && commandResult.Data != null)
                    {
                        return new ApiResponse<LoginResponse>
                        {
                            IsSuccess = true,
                            Data = commandResult.Data
                        };
                    }
                    else
                    {
                        var errorMsg = commandResult?.ErrorMessages?.FirstOrDefault() ?? "Login failed";
                        return new ApiResponse<LoginResponse>
                        {
                            IsSuccess = false,
                            ErrorMessage = errorMsg
                        };
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Login failed: {StatusCode} - {Error}", response.StatusCode, errorContent);

                    return new ApiResponse<LoginResponse>
                    {
                        IsSuccess = false,
                        ErrorMessage = $"Login failed: {response.StatusCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return new ApiResponse<LoginResponse>
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<ApiResponse<List<OrganizationViewModel>>> GetOrganizationsAsync(string token)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "/api/UserService/Organization/get-all");
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var queryResult = JsonSerializer.Deserialize<QueryResultWrapper<List<OrganizationViewModel>>>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (queryResult != null && queryResult.IsSuccess && queryResult.Data != null)
                    {
                        return new ApiResponse<List<OrganizationViewModel>>
                        {
                            IsSuccess = true,
                            Data = queryResult.Data
                        };
                    }
                    else
                    {
                        var errorMsg = queryResult?.ErrorMessage ?? "Failed to get organizations";
                        return new ApiResponse<List<OrganizationViewModel>>
                        {
                            IsSuccess = false,
                            ErrorMessage = errorMsg
                        };
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Get organizations failed: {StatusCode} - {Error}", response.StatusCode, errorContent);

                    return new ApiResponse<List<OrganizationViewModel>>
                    {
                        IsSuccess = false,
                        ErrorMessage = $"Failed to get organizations: {response.StatusCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting organizations");
                return new ApiResponse<List<OrganizationViewModel>>
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
