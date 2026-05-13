namespace Insurance.UserService.AppCore.Shared.Permissions.Commands.Create;

using Insurance.UserService.AppCore.Domain.Permissions.Enums;
using OysterFx.AppCore.Shared.Commands;
using System;

public class CreateSystemPermissionCommand : ICommand<Guid>
{
    public string? Code { get; set; }
    public string? Title { get; set; }
    public string? Module { get; set; }
    public PermissionType Type { get; set; } = PermissionType.Feature;
    public string? Description { get; set; }
}
