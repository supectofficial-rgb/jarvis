namespace Insurance.UserService.AppCore.Shared.Permissions.Commands.CreateApplicationPermission;

using OysterFx.AppCore.Shared.Commands;
using System;

public class CreateApplicationPermissionCommand : ICommand<Guid>
{
    public string? Code { get; set; }
    public string? Description { get; set; }
}