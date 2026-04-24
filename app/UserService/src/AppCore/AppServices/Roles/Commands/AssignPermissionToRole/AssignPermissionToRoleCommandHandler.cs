namespace Insurance.UserService.AppCore.AppServices.Roles.Commands.AssignPermissionToRole;

using Insurance.UserService.AppCore.Shared.Permissions.Commands;
using Insurance.UserService.AppCore.Shared.Roles.Commands;
using Insurance.UserService.AppCore.Shared.Roles.Commands.AssignPermissionToRole;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Domain.ValueObjects;
using OysterFx.AppCore.Shared.Commands.Common;

public class AssignPermissionToRoleCommandHandler : CommandHandler<AssignPermissionToRoleCommand, bool>
{
    private readonly IRoleCommandRepository _roleCommandRepository;
    private readonly IPermissionCommandRepository _permissionCommandRepository;

    public AssignPermissionToRoleCommandHandler(IRoleCommandRepository roleCommandRepository, IPermissionCommandRepository permissionCommandRepository)
    {
        _roleCommandRepository = roleCommandRepository;
        _permissionCommandRepository = permissionCommandRepository;
    }

    public override async Task<CommandResult<bool>> Handle(
        AssignPermissionToRoleCommand command)
    {
        var role = await _roleCommandRepository.GetAsync(BusinessKey.FromGuid(command.RoleBusinessKey));

        if (role is null)
            return CommandResult.Failure("Role not found");

        var permission = await _permissionCommandRepository.GetAsync(BusinessKey.FromGuid(command.PermissionBusinessKey));

        if (permission is null)
            return CommandResult.Failure("Permission not found");

        role.AssignPermission(permission);

        await _roleCommandRepository.CommitAsync();

        return await OkAsync(true);
    }
}