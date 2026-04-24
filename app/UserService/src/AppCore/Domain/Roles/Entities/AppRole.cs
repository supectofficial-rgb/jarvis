namespace Insurance.UserService.AppCore.Domain.Roles.Entities;

using Insurance.UserService.AppCore.Domain.Permissions.Entities;
using Insurance.UserService.AppCore.Domain.Roles.Enums;
using Insurance.UserService.AppCore.Domain.Tenants.Entities;
using Microsoft.AspNetCore.Identity;
using OysterFx.AppCore.Domain.Exceptions;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed class AppRole : IdentityRole<long>
{
    public TenantId TenantId { get; private set; }
    public BusinessKey BusinessKey { get; private set; }
    public RoleScope Scope { get; private set; }

    private readonly List<RolePermission> _permissions = new();
    public IReadOnlyCollection<RolePermission> Permissions => _permissions.AsReadOnly();

    private AppRole() { }

    private AppRole(string name, RoleScope scope, TenantId tenantId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name cannot be empty.", nameof(name));

        Name = name.Trim();
        Scope = scope;
        TenantId = tenantId;
        BusinessKey = BusinessKey.FromGuid(Guid.NewGuid());
        NormalizedName = Name.ToUpperInvariant();
    }

    public static AppRole ForSystem(string name, TenantId tenantId)
        => new(name, RoleScope.System, tenantId);

    public static AppRole ForOrganization(string name, TenantId tenantId)
        => new(name, RoleScope.Organization, tenantId);

    public static AppRole ForApplication(string name, TenantId tenantId)
        => new(name, RoleScope.Application, tenantId);

    public void AssignPermission(Permission permission)
    {
        if (permission is null)
            throw new ArgumentNullException(nameof(permission));

        if (!permission.IsActive)
            throw new AggregateStateExceptions("Cannot assign inactive permission.", nameof(permission.IsActive));

        if (permission.Scope != Scope)
            throw new AggregateStateExceptions($"Permission scope ({permission.Scope}) does not match role scope ({Scope}).", nameof(permission.Scope));

        if (_permissions.Any(x => x.PermissionBusinessKey == permission.BusinessKey))
            return;

        var rolePermission = RolePermission.Create(
            this.BusinessKey,
            permission.BusinessKey);

        _permissions.Add(rolePermission);
    }

    public void RemovePermission(BusinessKey permissionBusinessKey)
    {
        var existing = _permissions
            .FirstOrDefault(x => x.PermissionBusinessKey == permissionBusinessKey);

        if (existing != null)
            _permissions.Remove(existing);
    }
}