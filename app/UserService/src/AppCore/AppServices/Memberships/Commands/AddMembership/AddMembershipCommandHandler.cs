namespace Insurance.UserService.AppCore.AppServices.Memberships.Commands.AddMembership;

using Insurance.UserService.AppCore.Domain.Memberships.Entities;
using Insurance.UserService.AppCore.Shared.Memberships.Commands;
using Insurance.UserService.AppCore.Shared.Organizations.Commands;
using Insurance.UserService.AppCore.Shared.Users.Commands.AddMembership;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;
using System.Threading.Tasks;

public class AddMembershipCommandHandler(IOrganizationCommandRepository organizationCommandRepository, IMembershipCommandRepository membershipCommandRepository)
    : CommandHandler<AddMembershipCommand, Guid>
{
    private readonly IOrganizationCommandRepository _organizationCommandRepository = organizationCommandRepository;
    private readonly IMembershipCommandRepository _membershipCommandRepository = membershipCommandRepository;

    public override async Task<CommandResult<Guid>> Handle(AddMembershipCommand command)
    {
        var organization = await _organizationCommandRepository.GetAsync(command.OrganizationBusinessKey);
        var membership = Membership.Create(organization.TenantId, command.UserBusinessKey, command.OrganizationBusinessKey);
        await _membershipCommandRepository.InsertAsync(membership);
        await _membershipCommandRepository.CommitAsync();

        return await OkAsync(membership.BusinessKey.Value);
    }
}