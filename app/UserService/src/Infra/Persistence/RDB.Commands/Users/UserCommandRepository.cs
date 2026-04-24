namespace Insurance.UserService.Infra.Persistence.RDB.Commands.Users;

using Insurance.UserService.AppCore.Domain.Users.Entities;
using Insurance.UserService.AppCore.Shared.Users.Commands;
using Microsoft.EntityFrameworkCore;
using OysterFx.AppCore.Domain.ValueObjects;
using System.Threading.Tasks;

public class UserCommandRepository(InsuranceUserServiceDbContext dbContext) : IUserCommandRepository
{
    private readonly InsuranceUserServiceDbContext _dbContext = dbContext;

    public Task<int> CommitAsync()
    {
        return _dbContext.SaveChangesAsync();
    }

    public User Get(BusinessKey businessKey)
    {
        return _dbContext.Set<User>().FirstOrDefault(c => c.BusinessKey == businessKey)!;
    }

    public async Task<User> GetAsync(BusinessKey BusinessKey)
    {
        return (await _dbContext.Set<User>().FirstOrDefaultAsync(c => c.BusinessKey == BusinessKey))!;
    }

    public User GetByMobileNumber(string mobileNumber)
    {
        return _dbContext.Set<User>().FirstOrDefault(c => c.MobileNumber == mobileNumber)!;
    }

    public async Task InsertAsync(User entity)
    {
        await _dbContext.Set<User>().AddAsync(entity);
    }
}