namespace Insurance.UserService.AppCore.AppServices.Permissions.Commands.CreateOrganizationPermission;

using Insurance.UserService.AppCore.Domain.Permissions.Entities;
using Insurance.UserService.AppCore.Shared.Permissions.Commands;
using Insurance.UserService.AppCore.Shared.Permissions.Commands.CreateOrganizationPermission;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;
using System;
using System.Threading.Tasks;

public class CreateOrganizationPermissionCommandHandler : CommandHandler<CreateOrganizationPermissionCommand, Guid>
{
    private readonly IPermissionCommandRepository _permissionRepository;

    public CreateOrganizationPermissionCommandHandler(IPermissionCommandRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    public override async Task<CommandResult<Guid>> Handle(CreateOrganizationPermissionCommand command)
    {
        var permission = Permission.ForOrganization(command.Code!, command.Description);

        await _permissionRepository.InsertAsync(permission);
        await _permissionRepository.CommitAsync();

        return await OkAsync(permission.BusinessKey.Value);
    }
}