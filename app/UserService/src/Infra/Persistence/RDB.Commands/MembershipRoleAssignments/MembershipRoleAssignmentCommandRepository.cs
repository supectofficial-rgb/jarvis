namespace Insurance.UserService.Infra.Persistence.RDB.Commands.MembershipRoleAssignments;

using Insurance.UserService.AppCore.Domain.MembershipRoleAssignments.Entities;
using Insurance.UserService.AppCore.Shared.MembershipRoleAssignments.Commands;
using Microsoft.EntityFrameworkCore;
using OysterFx.AppCore.Domain.ValueObjects;
using System.Linq;
using System.Threading.Tasks;

public class MembershipRoleAssignmentCommandRepository(InsuranceUserServiceDbContext dbContext) : IMembershipRoleAssignmentCommandRepository
{
    private readonly InsuranceUserServiceDbContext _dbContext = dbContext;

    public Task<int> CommitAsync()
    {
        return _dbContext.SaveChangesAsync();
    }

    public MembershipRoleAssignment Get(BusinessKey businessKey)
    {
        return _dbContext.Set<MembershipRoleAssignment>().FirstOrDefault(c => c.BusinessKey == businessKey)!;
    }

    public async Task<MembershipRoleAssignment> GetAsync(BusinessKey BusinessKey)
    {
        return (await _dbContext.Set<MembershipRoleAssignment>().FirstOrDefaultAsync(c => c.BusinessKey == BusinessKey))!;
    }

    public async Task InsertAsync(MembershipRoleAssignment entity)
    {
        await _dbContext.Set<MembershipRoleAssignment>().AddAsync(entity);
    }
}