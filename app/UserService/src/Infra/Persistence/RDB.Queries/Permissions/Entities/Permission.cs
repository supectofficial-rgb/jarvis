namespace Insurance.UserService.Infra.Persistence.RDB.Queries.Permissions.Entities;

using Insurance.UserService.AppCore.Domain.Roles.Enums;
using System;

public class Permission
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public string? Code { get; set; }
    public RoleScope Scope { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}