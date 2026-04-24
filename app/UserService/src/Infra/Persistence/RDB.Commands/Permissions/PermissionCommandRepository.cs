namespace Insurance.UserService.Infra.Persistence.RDB.Commands.Permissions;

using Insurance.UserService.AppCore.Domain.Permissions.Entities;
using Insurance.UserService.AppCore.Shared.Permissions.Commands;
using Microsoft.EntityFrameworkCore;
using OysterFx.AppCore.Domain.ValueObjects;

public class PermissionCommandRepository(InsuranceUserServiceDbContext dbContext) : IPermissionCommandRepository
{
    private readonly InsuranceUserServiceDbContext _dbContext = dbContext;

    public Task<int> CommitAsync()
    {
        return _dbContext.SaveChangesAsync();
    }

    public Permission Get(BusinessKey businessKey)
    {
        return _dbContext.Set<Permission>().FirstOrDefault(c => c.BusinessKey == businessKey)!;
    }

    public async Task<Permission> GetAsync(BusinessKey BusinessKey)
    {
        return (await _dbContext.Set<Permission>().FirstOrDefaultAsync(c => c.BusinessKey == BusinessKey))!;
    }

    public async Task InsertAsync(Permission entity)
    {
        await _dbContext.Set<Permission>().AddAsync(entity);
    }
}