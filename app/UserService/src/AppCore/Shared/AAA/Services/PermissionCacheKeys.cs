namespace Insurance.UserService.AppCore.Shared.AAA.Services;

public static class PermissionCacheKeys
{
    private const string UserPermissionsPrefix = "user--permissions--";
    private const string RolePermissionsPrefix = "role--permissions--";

    public static string ForUserPermissions(Guid userBusinessKey)
        => $"{UserPermissionsPrefix}{userBusinessKey}";

    public static string ForRolePermissions(Guid roleBusinessKey)
        => $"{RolePermissionsPrefix}{roleBusinessKey}";
}
