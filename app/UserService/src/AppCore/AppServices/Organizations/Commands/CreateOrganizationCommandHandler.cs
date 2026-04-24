namespace Insurance.UserService.AppCore.AppServices.Organizations.Commands;

using Insurance.UserService.AppCore.Domain.Organizations.Entities;
using Insurance.UserService.AppCore.Shared.Organizations.Commands;
using Insurance.UserService.AppCore.Shared.Organizations.Commands.Create;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;
using System;
using System.Threading.Tasks;

public class CreateOrganizationCommandHandler(IOrganizationCommandRepository organizationCommandRepository) : CommandHandler<CreateOrganizationCommand, Guid>
{
    private readonly IOrganizationCommandRepository _organizationCommandRepository = organizationCommandRepository;

    public override async Task<CommandResult<Guid>> Handle(CreateOrganizationCommand command)
    {
        var newOrganization = Organization.NewOrganization(command.Title!);
        await _organizationCommandRepository.InsertAsync(newOrganization);
        await _organizationCommandRepository.CommitAsync();

        return await OkAsync(newOrganization.BusinessKey.Value);
    }
}