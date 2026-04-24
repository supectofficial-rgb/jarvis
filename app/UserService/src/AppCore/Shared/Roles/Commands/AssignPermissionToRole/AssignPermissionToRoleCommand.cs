namespace Insurance.UserService.AppCore.Shared.Roles.Commands.AssignPermissionToRole;

using OysterFx.AppCore.Shared.Commands;
using System;

public class AssignPermissionToRoleCommand : ICommand<bool>
{
    public Guid RoleBusinessKey { get; set; }
    public Guid PermissionBusinessKey { get; set; }
}