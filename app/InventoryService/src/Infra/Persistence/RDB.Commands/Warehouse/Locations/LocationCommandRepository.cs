namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Warehouse.Locations;

using Insurance.InventoryService.AppCore.Domain.Warehouse.Entities;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands;
using Microsoft.EntityFrameworkCore;
using OysterFx.AppCore.Domain.ValueObjects;
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
            .FirstOrDefaultAsync(x => x.BusinessKey == BusinessKey.FromGuid(locationBusinessKey));
    }

    public Task<bool> ExistsByCodeAsync(string locationCode, Guid? exceptBusinessKey = null)
    {
        var normalized = locationCode.Trim();
        var query = _dbContext.Set<Location>().Where(x => x.LocationCode == normalized);

        if (exceptBusinessKey.HasValue)
            query = query.Where(x => x.BusinessKey != BusinessKey.FromGuid(exceptBusinessKey.Value));

        return query.AnyAsync();
    }

    public async Task ReplaceStructureSelectionsAsync(Guid locationBusinessKey, IReadOnlyCollection<LocationStructureSelectionItem> selections)
    {
        var existingSelections = await _dbContext.Set<LocationStructureSelection>()
            .Where(x => x.LocationRef == locationBusinessKey)
            .ToListAsync();

        if (existingSelections.Count > 0)
        {
            _dbContext.RemoveRange(existingSelections);
        }

        var normalizedSelections = (selections ?? Array.Empty<LocationStructureSelectionItem>())
            .Where(x => x.StructureRef != Guid.Empty && x.StructureValueRef != Guid.Empty)
            .GroupBy(x => x.StructureRef)
            .Select(x => x.Last())
            .ToList();

        if (normalizedSelections.Count == 0)
        {
            return;
        }

        var aggregates = normalizedSelections
            .Select(x => LocationStructureSelection.Create(locationBusinessKey, x.StructureRef, x.StructureValueRef))
            .ToList();

        await _dbContext.Set<LocationStructureSelection>().AddRangeAsync(aggregates);
    }
}
