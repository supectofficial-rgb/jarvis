using Insurance.InventoryDashboard.Panel.Models;

namespace Insurance.InventoryDashboard.Panel.Services;

public sealed class AuthApiService : IAuthApiService
{
    private readonly IApiService _apiService;

    public AuthApiService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public Task<ApiResponse<LoginResponse>> LoginAsync(string userName, string password) =>
        _apiService.LoginAsync(userName, password);

    public Task<ApiResponse<List<OrganizationViewModel>>> GetOrganizationsAsync(string token) =>
        _apiService.GetOrganizationsAsync(token);
}
