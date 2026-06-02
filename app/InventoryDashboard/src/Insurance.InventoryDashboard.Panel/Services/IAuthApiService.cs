using Insurance.InventoryDashboard.Panel.Models;

namespace Insurance.InventoryDashboard.Panel.Services;

public interface IAuthApiService
{
    Task<ApiResponse<LoginResponse>> LoginAsync(string userName, string password);
    Task<ApiResponse<bool>> LogoutAsync(string accessToken, string refreshToken, string? reason = null);
    Task<ApiResponse<List<OrganizationViewModel>>> GetOrganizationsAsync(string token);
}
