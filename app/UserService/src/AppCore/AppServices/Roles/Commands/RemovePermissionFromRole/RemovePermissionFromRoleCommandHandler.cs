namespace Insurance.UserService.AppCore.AppServices.Roles.Commands.RemovePermissionFromRole;

using Insurance.UserService.AppCore.Shared.Roles.Commands;
using Insurance.UserService.AppCore.Shared.Roles.Commands.RemovePermissionFromRole;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Domain.ValueObjects;
using OysterFx.AppCore.Shared.Commands.Common;
using System.Threading.Tasks;

public class RemovePermissionFromRoleCommandHandler : CommandHandler<RemovePermissionFromRoleCommand, bool>
{
    private readonly IRoleCommandRepository _roleRepository;

    public RemovePermissionFromRoleCommandHandler(
        IRoleCommandRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public override async Task<CommandResult<bool>> Handle(
        RemovePermissionFromRoleCommand command)
    {
        var role = await _roleRepository.GetAsync(BusinessKey.FromGuid(command.RoleBusinessKey));

        if (role is null)
            return CommandResult<bool>.Failure("Role not found");

        role.RemovePermission(
            BusinessKey.FromGuid(command.PermissionBusinessKey));

        await _roleRepository.CommitAsync();

        return await OkAsync(true);
    }
}