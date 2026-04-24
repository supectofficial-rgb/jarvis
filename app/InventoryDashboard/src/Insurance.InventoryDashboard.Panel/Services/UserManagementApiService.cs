using Insurance.InventoryDashboard.Panel.Models;

namespace Insurance.InventoryDashboard.Panel.Services;

public sealed class UserManagementApiService : IUserManagementApiService
{
    private readonly IApiService _apiService;

    public UserManagementApiService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public Task<ApiResponse<List<UserSummaryModel>>> GetUsersAsync(string token) =>
        _apiService.GetUsersAsync(token);

    public Task<ApiResponse<List<PersonaSummaryModel>>> GetPersonasAsync(string token) =>
        _apiService.GetPersonasAsync(token);

    public Task<ApiResponse<List<PermissionSummaryModel>>> GetPermissionsAsync(string token) =>
        _apiService.GetPermissionsAsync(token);

    public Task<ApiResponse<bool>> AssignPersonaToUserAsync(string userId, string personaId, string token) =>
        _apiService.AssignPersonaToUserAsync(userId, personaId, token);

    public Task<ApiResponse<bool>> RemovePersonaFromUserAsync(string userId, string personaId, string token) =>
        _apiService.RemovePersonaFromUserAsync(userId, personaId, token);

    public Task<ApiResponse<bool>> AssignPermissionToPersonaAsync(string personaId, string permissionId, string token) =>
        _apiService.AssignPermissionToPersonaAsync(personaId, permissionId, token);

    public Task<ApiResponse<bool>> RemovePermissionFromPersonaAsync(string personaId, string permissionId, string token) =>
        _apiService.RemovePermissionFromPersonaAsync(personaId, permissionId, token);
}
