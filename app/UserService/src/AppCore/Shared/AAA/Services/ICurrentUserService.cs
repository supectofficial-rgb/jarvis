namespace Insurance.UserService.AppCore.Shared.AAA.Services;

using Insurance.UserService.AppCore.Domain.Users.Dtos;
using OysterFx.AppCore.Domain.ValueObjects;

public interface ICurrentUserService
{
    long? UserId { get; }
    BusinessKey? UserBusinessKey { get; }
    BusinessKey? CurrentOrganizationKey { get; }
    BusinessKey? CurrentPersonaKey { get; }
    bool IsAuthenticated { get; }
    Task<UserContext> GetCurrentUserContextAsync();
    Task<bool> HasPermissionAsync(string permissionCode);
    Task<List<string>> GetCurrentUserPermissionsAsync();
}
