namespace Insurance.UserService.Infra.Persistence.RDB.Commands.OtpCodes;

using Insurance.UserService.AppCore.Domain.OtpCodes.Entities;
using Insurance.UserService.AppCore.Shared.OtpCodeContracts.OtpCodes.Commands;
using Microsoft.EntityFrameworkCore;

public class OtpCodeCommandRepository : IOtpCodeCommandRepository
{
    private readonly InsuranceUserServiceDbContext _dbContext;

    public OtpCodeCommandRepository(InsuranceUserServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<OtpCode?> GetByMobileNumberAndCode(string mobileNumber, string code)
    {
        return await _dbContext.Set<OtpCode>()
            .Where(c => c.MobileNumber == mobileNumber)
            .OrderByDescending(c => c.CreatedAt)
            .FirstOrDefaultAsync(c => c.Code == code);
    }

    public async Task<OtpCode?> GetLatestByMobileNumberAsync(string mobileNumber)
    {
        return await _dbContext.Set<OtpCode>()
            .Where(c => c.MobileNumber == mobileNumber)
            .OrderByDescending(c => c.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task InsertAsync(OtpCode entity)
    {
        await _dbContext.Set<OtpCode>().AddAsync(entity);
    }

    public Task<int> CommitAsync()
    {
        return _dbContext.SaveChangesAsync();
    }
}
