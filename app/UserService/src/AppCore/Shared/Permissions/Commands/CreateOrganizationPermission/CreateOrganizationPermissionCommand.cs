namespace Insurance.UserService.AppCore.Shared.Permissions.Commands.CreateOrganizationPermission;

using OysterFx.AppCore.Shared.Commands;
using System;

public class CreateOrganizationPermissionCommand : ICommand<Guid>
{
    public string? Code { get; set; }
    public string? Description { get; set; }
}