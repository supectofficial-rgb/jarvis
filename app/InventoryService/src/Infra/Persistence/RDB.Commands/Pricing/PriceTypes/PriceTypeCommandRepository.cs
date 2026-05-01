namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Pricing.PriceTypes;

using Insurance.InventoryService.AppCore.Domain.Pricing.Entities;
using Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Commands;
using Microsoft.EntityFrameworkCore;
using OysterFx.AppCore.Domain.ValueObjects;
using OysterFx.Infra.Persistence.RDB.Commands;

public class PriceTypeCommandRepository : CommandRepository<PriceType, InventoryServiceCommandDbContext>, IPriceTypeCommandRepository
{
    public PriceTypeCommandRepository(InventoryServiceCommandDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<PriceType?> GetByBusinessKeyAsync(Guid priceTypeBusinessKey)
    {
        return _dbContext.Set<PriceType>()
            .FirstOrDefaultAsync(x => x.BusinessKey == BusinessKey.FromGuid(priceTypeBusinessKey));
    }

    public Task<bool> ExistsByCodeAsync(string code, Guid? exceptBusinessKey = null)
    {
        var normalized = code.Trim();
        var query = _dbContext.Set<PriceType>().Where(x => x.Code == normalized);

        if (exceptBusinessKey.HasValue)
            query = query.Where(x => x.BusinessKey != BusinessKey.FromGuid(exceptBusinessKey.Value));

        return query.AnyAsync();
    }
}
