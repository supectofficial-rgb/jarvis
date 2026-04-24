namespace Insurance.UserService.Infra.Persistence.RDB.Commands.Organizations;

using Insurance.UserService.AppCore.Domain.Organizations.Entities;
using Insurance.UserService.AppCore.Shared.Organizations.Commands;
using Microsoft.EntityFrameworkCore;
using OysterFx.AppCore.Domain.ValueObjects;

public class OrganizationCommandRepository(InsuranceUserServiceDbContext dbContext) : IOrganizationCommandRepository
{
    private readonly InsuranceUserServiceDbContext _dbContext = dbContext;

    public Task<int> CommitAsync()
    {
        return _dbContext.SaveChangesAsync();
    }

    public Organization Get(BusinessKey businessKey)
    {
        return _dbContext.Set<Organization>().FirstOrDefault(c => c.BusinessKey == businessKey)!;
    }

    public async Task<Organization> GetAsync(BusinessKey BusinessKey)
    {
        return (await _dbContext.Set<Organization>().FirstOrDefaultAsync(c => c.BusinessKey == BusinessKey))!;
    }

    public async Task InsertAsync(Organization entity)
    {
        await _dbContext.Set<Organization>().AddAsync(entity);
    }
}