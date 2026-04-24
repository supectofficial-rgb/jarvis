namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Warehouse.Warehouses;

using Insurance.InventoryService.AppCore.Domain.Warehouse.Entities;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Commands;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Commands;

public class WarehouseCommandRepository : CommandRepository<Warehouse, InventoryServiceCommandDbContext>, IWarehouseCommandRepository
{
    public WarehouseCommandRepository(InventoryServiceCommandDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<Warehouse?> GetByBusinessKeyAsync(Guid warehouseBusinessKey)
    {
        return _dbContext.Set<Warehouse>()
            .FirstOrDefaultAsync(x => x.BusinessKey.Value == warehouseBusinessKey);
    }

    public Task<bool> ExistsByCodeAsync(string code, Guid? exceptBusinessKey = null)
    {
        var normalized = code.Trim();
        var query = _dbContext.Set<Warehouse>().Where(x => x.Code == normalized);

        if (exceptBusinessKey.HasValue)
            query = query.Where(x => x.BusinessKey.Value != exceptBusinessKey.Value);

        return query.AnyAsync();
    }
}
