using Insurance.InventoryDashboard.Panel.Models;

namespace Insurance.InventoryDashboard.Panel.Services;

public interface IUserManagementApiService
{
    Task<ApiResponse<List<UserSummaryModel>>> GetUsersAsync(string token);
    Task<ApiResponse<List<PersonaSummaryModel>>> GetPersonasAsync(string token);
    Task<ApiResponse<List<PermissionSummaryModel>>> GetPermissionsAsync(string token);

    Task<ApiResponse<bool>> AssignPersonaToUserAsync(string userId, string personaId, string token);
    Task<ApiResponse<bool>> RemovePersonaFromUserAsync(string userId, string personaId, string token);
    Task<ApiResponse<bool>> AssignPermissionToPersonaAsync(string personaId, string permissionId, string token);
    Task<ApiResponse<bool>> RemovePermissionFromPersonaAsync(string personaId, string permissionId, string token);
}
