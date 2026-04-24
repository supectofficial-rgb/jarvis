namespace Insurance.UserService.Infra.Persistence.RDB.Commands.Roles;

using Insurance.UserService.AppCore.Domain.Roles.Entities;
using Insurance.UserService.AppCore.Shared.Roles.Commands;
using Microsoft.EntityFrameworkCore;
using OysterFx.AppCore.Domain.ValueObjects;
using System.Threading.Tasks;

public class RoleCommandRepository(InsuranceUserServiceDbContext dbContext) : IRoleCommandRepository
{
    private readonly InsuranceUserServiceDbContext _dbContext = dbContext;

    public Task<int> CommitAsync()
    {
        return _dbContext.SaveChangesAsync();
    }

    public AppRole Get(BusinessKey businessKey)
    {
        return _dbContext.Set<AppRole>().FirstOrDefault(c => c.BusinessKey == businessKey)!;
    }

    public async Task<AppRole> GetAsync(BusinessKey BusinessKey)
    {
        return (await _dbContext.Set<AppRole>().FirstOrDefaultAsync(c => c.BusinessKey == BusinessKey))!;
    }

    public async Task InsertAsync(AppRole entity)
    {
        await _dbContext.Set<AppRole>().AddAsync(entity);
    }
}