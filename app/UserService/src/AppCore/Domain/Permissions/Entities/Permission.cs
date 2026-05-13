namespace Insurance.UserService.AppCore.Domain.Permissions.Entities;

using Insurance.UserService.AppCore.Domain.Permissions.Enums;
using Insurance.UserService.AppCore.Domain.Roles.Enums;
using OysterFx.AppCore.Domain.Aggregates;

public class Permission : AggregateRoot
{
    public string Code { get; private set; }
    public string Title { get; private set; }
    public string Module { get; private set; }
    public PermissionType Type { get; private set; }
    public RoleScope Scope { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    private Permission() { }

    private Permission(
        string code,
        RoleScope scope,
        string? description = null,
        string? title = null,
        string? module = null,
        PermissionType type = PermissionType.Feature)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Permission code cannot be empty.", nameof(code));

        Code = Normalize(code);
        Title = ResolveTitle(title, code);
        Module = ResolveModule(module, Code);
        Type = type == PermissionType.Unknown ? PermissionType.Feature : type;
        Scope = scope;
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        IsActive = true;
    }

    public static Permission ForSystem(
        string code,
        string? description = null,
        string? title = null,
        string? module = null,
        PermissionType type = PermissionType.Feature) =>
        new(code, RoleScope.System, description, title, module, type);

    public static Permission ForOrganization(
        string code,
        string? description = null,
        string? title = null,
        string? module = null,
        PermissionType type = PermissionType.Feature) =>
        new(code, RoleScope.Organization, description, title, module, type);

    public static Permission ForApplication(
        string code,
        string? description = null,
        string? title = null,
        string? module = null,
        PermissionType type = PermissionType.Feature) =>
        new(code, RoleScope.Application, description, title, module, type);

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;

    private static string Normalize(string code) => code.Trim().ToUpperInvariant();

    private static string ResolveTitle(string? title, string code) =>
        string.IsNullOrWhiteSpace(title) ? code.Trim() : title.Trim();

    private static string ResolveModule(string? module, string code)
    {
        if (!string.IsNullOrWhiteSpace(module))
            return module.Trim();

        var parts = code.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return parts.Length switch
        {
            0 => code,
            1 => parts[0],
            _ => $"{parts[0]}.{parts[1]}"
        };
    }
}
