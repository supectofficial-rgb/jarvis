namespace Insurance.UserService.AppCore.Shared.Roles.Commands.RemovePermissionFromRole;

using OysterFx.AppCore.Shared.Commands;
using System;

public class RemovePermissionFromRoleCommand : ICommand<bool>
{
    public Guid RoleBusinessKey { get; set; }
    public Guid PermissionBusinessKey { get; set; }
}