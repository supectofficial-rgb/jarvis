namespace Insurance.UserService.Infra.Persistence.RDB.Commands.Memberships;

using Insurance.UserService.AppCore.Domain.Memberships.Entities;
using Insurance.UserService.AppCore.Shared.Memberships.Commands;
using Microsoft.EntityFrameworkCore;
using OysterFx.AppCore.Domain.ValueObjects;

public class MembershipCommandRepository(InsuranceUserServiceDbContext dbContext) : IMembershipCommandRepository
{
    private readonly InsuranceUserServiceDbContext _dbContext = dbContext;

    public Task<int> CommitAsync()
    {
        return _dbContext.SaveChangesAsync();
    }

    public Membership Get(BusinessKey businessKey)
    {
        return _dbContext.Set<Membership>().FirstOrDefault(c => c.BusinessKey == businessKey)!;
    }

    public async Task<Membership> GetAsync(BusinessKey BusinessKey)
    {
        return (await _dbContext.Set<Membership>().FirstOrDefaultAsync(c => c.BusinessKey == BusinessKey))!;
    }

    public async Task InsertAsync(Membership entity)
    {
        await _dbContext.Set<Membership>().AddAsync(entity);
    }
}