namespace Insurance.UserService.AppCore.AppServices.Permissions.Commands.CreateSystemPermission;

using Insurance.UserService.AppCore.Domain.Permissions.Entities;
using Insurance.UserService.AppCore.Shared.Permissions.Commands;
using Insurance.UserService.AppCore.Shared.Permissions.Commands.Create;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class CreateSystemPermissionCommandHandler : CommandHandler<CreateSystemPermissionCommand, Guid>
{
    private readonly IPermissionCommandRepository _permissionRepository;

    public CreateSystemPermissionCommandHandler(IPermissionCommandRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    public override async Task<CommandResult<Guid>> Handle(
        CreateSystemPermissionCommand command)
    {
        var permission = Permission.ForSystem(command.Code!, command.Description);

        await _permissionRepository.InsertAsync(permission);
        await _permissionRepository.CommitAsync();

        return await OkAsync(permission.BusinessKey.Value);
    }
}