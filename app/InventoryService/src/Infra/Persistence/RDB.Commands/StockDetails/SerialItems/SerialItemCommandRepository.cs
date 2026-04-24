namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.StockDetails.SerialItems;

using Insurance.InventoryService.AppCore.Domain.StockDetails.Entities;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Commands;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Commands;

public class SerialItemCommandRepository : CommandRepository<SerialItem, InventoryServiceCommandDbContext>, ISerialItemCommandRepository
{
    public SerialItemCommandRepository(InventoryServiceCommandDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<SerialItem?> GetByBusinessKeyAsync(Guid serialItemBusinessKey)
    {
        return _dbContext.Set<SerialItem>()
            .FirstOrDefaultAsync(x => x.BusinessKey.Value == serialItemBusinessKey);
    }

    public Task<bool> ExistsBySerialNoAsync(Guid variantRef, string serialNo, Guid? exceptBusinessKey = null)
    {
        if (variantRef == Guid.Empty || string.IsNullOrWhiteSpace(serialNo))
            return Task.FromResult(false);

        var normalized = serialNo.Trim();
        var query = _dbContext.Set<SerialItem>()
            .Where(x => x.VariantRef == variantRef && x.SerialNo == normalized);

        if (exceptBusinessKey.HasValue)
            query = query.Where(x => x.BusinessKey.Value != exceptBusinessKey.Value);

        return query.AnyAsync();
    }
}
