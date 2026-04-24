namespace Insurance.UserService.AppCore.AppServices.MembershipRoleAssignments.Commands.AssignRoleToMembership;

using Insurance.UserService.AppCore.Domain.MembershipRoleAssignments.Entities;
using Insurance.UserService.AppCore.Shared.MembershipRoleAssignments.Commands;
using Insurance.UserService.AppCore.Shared.Users.Commands.AssignRoleToMembership;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;
using System.Threading.Tasks;

public class AssignRoleToMembershipCommandHandler(IMembershipRoleAssignmentCommandRepository membershipRoleAssignmentCommandRepository) : CommandHandler<AssignRoleToMembershipCommand, bool>
{
    private readonly IMembershipRoleAssignmentCommandRepository _membershipRoleAssignmentCommandRepository = membershipRoleAssignmentCommandRepository;

    public override async Task<CommandResult<bool>> Handle(AssignRoleToMembershipCommand command)
    {
        var membershipRoleAssignment = MembershipRoleAssignment.Create(command.MembershipBusinessKey, command.RoleBusinessKey);
        await _membershipRoleAssignmentCommandRepository.InsertAsync(membershipRoleAssignment);
        await _membershipRoleAssignmentCommandRepository.CommitAsync();

        return await OkAsync(true);
    }
}