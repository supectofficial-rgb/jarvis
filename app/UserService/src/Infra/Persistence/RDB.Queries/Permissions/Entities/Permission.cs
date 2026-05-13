namespace Insurance.UserService.Infra.Persistence.RDB.Queries.Permissions.Entities;

using Insurance.UserService.AppCore.Domain.Permissions.Enums;
using Insurance.UserService.AppCore.Domain.Roles.Enums;
using System;

public class Permission
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public string? Code { get; set; }
    public string? Title { get; set; }
    public string? Module { get; set; }
    public PermissionType Type { get; set; }
    public RoleScope Scope { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}
