namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Pricing.PriceChannels;

using Insurance.InventoryService.AppCore.Domain.Pricing.Entities;
using Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Commands;
using Microsoft.EntityFrameworkCore;
using OysterFx.AppCore.Domain.ValueObjects;
using OysterFx.Infra.Persistence.RDB.Commands;

public class PriceChannelCommandRepository : CommandRepository<PriceChannel, InventoryServiceCommandDbContext>, IPriceChannelCommandRepository
{
    public PriceChannelCommandRepository(InventoryServiceCommandDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<PriceChannel?> GetByBusinessKeyAsync(Guid priceChannelBusinessKey)
    {
        return _dbContext.Set<PriceChannel>()
            .FirstOrDefaultAsync(x => x.BusinessKey == BusinessKey.FromGuid(priceChannelBusinessKey));
    }

    public Task<bool> ExistsByCodeAsync(string code, Guid? exceptBusinessKey = null)
    {
        var normalized = code.Trim();
        var query = _dbContext.Set<PriceChannel>().Where(x => x.Code == normalized);

        if (exceptBusinessKey.HasValue)
            query = query.Where(x => x.BusinessKey != BusinessKey.FromGuid(exceptBusinessKey.Value));

        return query.AnyAsync();
    }
}
