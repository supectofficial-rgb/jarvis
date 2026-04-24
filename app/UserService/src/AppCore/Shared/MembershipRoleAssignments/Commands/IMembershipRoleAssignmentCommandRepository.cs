namespace Insurance.UserService.AppCore.Shared.MembershipRoleAssignments.Commands;

using Insurance.UserService.AppCore.Domain.MembershipRoleAssignments.Entities;
using OysterFx.AppCore.Domain.ValueObjects;
using System.Threading.Tasks;

public interface IMembershipRoleAssignmentCommandRepository
{
    MembershipRoleAssignment Get(BusinessKey BusinessKey);
    Task<MembershipRoleAssignment> GetAsync(BusinessKey BusinessKey);
    Task InsertAsync(MembershipRoleAssignment entity);
    Task<int> CommitAsync();
}