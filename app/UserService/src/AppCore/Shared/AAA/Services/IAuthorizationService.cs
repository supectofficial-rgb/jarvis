using OysterFx.AppCore.Domain.ValueObjects;

namespace Insurance.UserService.AppCore.Shared.AAA.Services;

public interface IAuthorizationService
{

    Task<bool> HasPermissionAsync(long userId, string permissionCode, BusinessKey organizationKey);
    Task<IEnumerable<string>> GetUserPermissionsAsync(long userId, BusinessKey personaKey);
    Task<bool> IsUserInRoleAsync(long userId, string roleName, BusinessKey personaKey);
}
