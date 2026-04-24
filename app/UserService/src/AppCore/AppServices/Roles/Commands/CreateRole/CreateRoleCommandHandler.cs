namespace Insurance.UserService.AppCore.AppServices.Roles.Commands.CreateRole;

using Insurance.UserService.AppCore.Domain.Roles.Entities;
using Insurance.UserService.AppCore.Domain.Roles.Enums;
using Insurance.UserService.AppCore.Domain.Tenants.Entities;
using Insurance.UserService.AppCore.Shared.AAA.Services;
using Insurance.UserService.AppCore.Shared.Roles.Commands;
using Insurance.UserService.AppCore.Shared.Roles.Commands.CreateRole;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;
using System;
using System.Threading.Tasks;

public class CreateRoleCommandHandler : CommandHandler<CreateRoleCommand, Guid>
{
    private readonly IRoleCommandRepository _roleRepository;
    private readonly IUserContextService _userContextService;

    public CreateRoleCommandHandler(
        IRoleCommandRepository roleRepository,
        IUserContextService userContext)
    {
        _roleRepository = roleRepository;
        _userContextService = userContext;
    }

    public override async Task<CommandResult<Guid>> Handle(
        CreateRoleCommand command)
    {
        // TODO: Must remove .Result
        // var tenantId = _userContextService.GetCurrentUserContextAsync().Result!.TenantId;
        var tenantId = TenantId.FromString("TENANT_ee3a6e476c5c4fdeb3deba60c8460309") ;

        AppRole role = command.Scope switch
        {
            RoleScope.System => AppRole.ForSystem(command.Name!, tenantId),
            RoleScope.Organization => AppRole.ForOrganization(command.Name!, tenantId),
            RoleScope.Application => AppRole.ForApplication(command.Name!, tenantId),
            _ => AppRole.ForSystem(command.Name!, tenantId)
        };

        await _roleRepository.InsertAsync(role);
        await _roleRepository.CommitAsync();

        return await OkAsync(role.BusinessKey.Value);
    }
}
