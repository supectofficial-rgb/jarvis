namespace Insurance.UserService.AppCore.AppServices.Permissions.Commands.CreateApplicationPermission;

using Insurance.UserService.AppCore.Domain.Permissions.Entities;
using Insurance.UserService.AppCore.Shared.Permissions.Commands;
using Insurance.UserService.AppCore.Shared.Permissions.Commands.CreateApplicationPermission;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class CreateApplicationPermissionCommandHandler : CommandHandler<CreateApplicationPermissionCommand, Guid>
{
    private readonly IPermissionCommandRepository _permissionRepository;

    public CreateApplicationPermissionCommandHandler(IPermissionCommandRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    public override async Task<CommandResult<Guid>> Handle(CreateApplicationPermissionCommand command)
    {
        var permission = Permission.ForApplication(command.Code!, command.Description);

        await _permissionRepository.InsertAsync(permission);
        await _permissionRepository.CommitAsync();

        return await OkAsync(permission.BusinessKey.Value);
    }
}