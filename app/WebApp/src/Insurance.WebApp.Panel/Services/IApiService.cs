using Insurance.WebApp.Panel.Models;

namespace Insurance.WebApp.Panel.Services
{
    public interface IApiService
    {
        Task<ApiResponse<LoginResponse>> LoginAsync(string userName, string password);
        Task<ApiResponse<List<OrganizationViewModel>>> GetOrganizationsAsync(string token);
    }
}

