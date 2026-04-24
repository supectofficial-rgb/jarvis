namespace Insurance.UserService.AppCore.Shared.Roles.Commands.CreateRole;

using Insurance.UserService.AppCore.Domain.Roles.Enums;
using OysterFx.AppCore.Shared.Commands;
using System;

public class CreateRoleCommand : ICommand<Guid>
{
    public string? Name { get; set; }
    public RoleScope Scope { get; set; }
}