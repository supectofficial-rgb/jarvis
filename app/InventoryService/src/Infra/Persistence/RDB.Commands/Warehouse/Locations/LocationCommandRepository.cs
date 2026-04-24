namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Warehouse.Locations;

using Insurance.InventoryService.AppCore.Domain.Warehouse.Entities;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Commands;

public class LocationCommandRepository : CommandRepository<Location, InventoryServiceCommandDbContext>, ILocationCommandRepository
{
    public LocationCommandRepository(InventoryServiceCommandDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<Location?> GetByBusinessKeyAsync(Guid locationBusinessKey)
    {
        return _dbContext.Set<Location>()
            .FirstOrDefaultAsync(x => x.BusinessKey.Value == locationBusinessKey);
    }

    public Task<bool> ExistsByCodeAsync(string locationCode, Guid? exceptBusinessKey = null)
    {
        var normalized = locationCode.Trim();
        var query = _dbContext.Set<Location>().Where(x => x.LocationCode == normalized);

        if (exceptBusinessKey.HasValue)
            query = query.Where(x => x.BusinessKey.Value != exceptBusinessKey.Value);

        return query.AnyAsync();
    }
}
