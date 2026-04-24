namespace Insurance.UserService.AppCore.Domain.Roles.Entities;

using OysterFx.AppCore.Domain.Aggregates;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed class RolePermission : Entity<long>
{
    public BusinessKey RoleBusinessKey { get; private set; }
    public BusinessKey PermissionBusinessKey { get; private set; }

    private RolePermission() { }

    internal static RolePermission Create(
        BusinessKey roleBusinessKey,
        BusinessKey permissionBusinessKey)
    {
        return new RolePermission
        {
            RoleBusinessKey = roleBusinessKey,
            PermissionBusinessKey = permissionBusinessKey
        };
    }
}