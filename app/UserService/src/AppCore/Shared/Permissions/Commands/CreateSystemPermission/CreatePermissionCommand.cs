namespace Insurance.UserService.AppCore.Shared.Permissions.Commands.Create;

using OysterFx.AppCore.Shared.Commands;
using System;

public class CreateSystemPermissionCommand : ICommand<Guid>
{
    public string? Code { get; set; }
    public string? Description { get; set; }
}