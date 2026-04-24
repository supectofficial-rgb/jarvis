namespace Insurance.UserService.AppCore.Domain.Permissions.Entities;

using Insurance.UserService.AppCore.Domain.Roles.Enums;
using OysterFx.AppCore.Domain.Aggregates;

public class Permission : AggregateRoot
{
    public string Code { get; private set; }
    public RoleScope Scope { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    private Permission() { }

    private Permission(string code, RoleScope scope, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Permission code cannot be empty.", nameof(code));

        Code = Normalize(code);
        Scope = scope;
        Description = description;
        IsActive = true;
    }

    public static Permission ForSystem(string code, string? description = null) => new(code, RoleScope.System, description);

    public static Permission ForOrganization(string code, string? description = null) => new(code, RoleScope.Organization, description);

    public static Permission ForApplication(string code, string? description = null) => new(code, RoleScope.Application, description);

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;

    private static string Normalize(string code) => code.Trim().ToUpperInvariant();
}