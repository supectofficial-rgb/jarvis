namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Warehouse.LocationStructures;

using Insurance.InventoryService.AppCore.Domain.Warehouse.Entities;
using Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Commands;
using Microsoft.EntityFrameworkCore;
using OysterFx.AppCore.Domain.ValueObjects;
using OysterFx.Infra.Persistence.RDB.Commands;

public sealed class LocationStructureCommandRepository : CommandRepository<LocationStructureNode, InventoryServiceCommandDbContext>, ILocationStructureCommandRepository
{
    public LocationStructureCommandRepository(InventoryServiceCommandDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task InsertAsync(LocationStructureValue entity)
    {
        await _dbContext.Set<LocationStructureValue>().AddAsync(entity);
    }

    public Task<LocationStructureNode?> GetNodeByBusinessKeyAsync(Guid structureBusinessKey)
    {
        return _dbContext.Set<LocationStructureNode>()
            .FirstOrDefaultAsync(x => x.BusinessKey == BusinessKey.FromGuid(structureBusinessKey));
    }

    public Task<LocationStructureValue?> GetValueByBusinessKeyAsync(Guid valueBusinessKey)
    {
        return _dbContext.Set<LocationStructureValue>()
            .FirstOrDefaultAsync(x => x.BusinessKey == BusinessKey.FromGuid(valueBusinessKey));
    }

    public Task<bool> ExistsNodeCodeAsync(Guid warehouseRef, string code, Guid? excludedBusinessKey = null)
    {
        var normalized = code.Trim();
        var query = _dbContext.Set<LocationStructureNode>().Where(x => x.WarehouseRef == warehouseRef && x.Code == normalized);
        if (excludedBusinessKey.HasValue)
        {
            var excluded = BusinessKey.FromGuid(excludedBusinessKey.Value);
            query = query.Where(x => x.BusinessKey != excluded);
        }

        return query.AnyAsync();
    }

    public Task<bool> ExistsValueCodeAsync(Guid structureRef, string code, Guid? excludedBusinessKey = null)
    {
        var normalized = code.Trim();
        var query = _dbContext.Set<LocationStructureValue>().Where(x => x.StructureRef == structureRef && x.Code == normalized);
        if (excludedBusinessKey.HasValue)
        {
            var excluded = BusinessKey.FromGuid(excludedBusinessKey.Value);
            query = query.Where(x => x.BusinessKey != excluded);
        }

        return query.AnyAsync();
    }
}
