namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Warehouse.LocationStructures;

using Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Queries;
using Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Queries.GetLocationStructureValues;
using Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Queries.GetWarehouseLocationStructureTree;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Warehouse.LocationStructures.Entities;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Queries;

public sealed class LocationStructureQueryRepository : QueryRepository<InventoryServiceQueryDbContext>, ILocationStructureQueryRepository
{
    public LocationStructureQueryRepository(InventoryServiceQueryDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<GetWarehouseLocationStructureTreeQueryResult?> GetTreeAsync(Guid warehouseRef, bool includeInactive = false)
    {
        var nodes = await _dbContext.Set<LocationStructureNodeReadModel>()
            .Where(x => x.WarehouseRef == warehouseRef)
            .Where(x => includeInactive || x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync();

        var values = await _dbContext.Set<LocationStructureValueReadModel>()
            .Where(x => nodes.Select(n => n.BusinessKey).Contains(x.StructureRef))
            .Where(x => includeInactive || x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync();

        var items = nodes.ToDictionary(
            x => x.BusinessKey,
            x => new LocationStructureTreeItem
            {
                LocationStructureBusinessKey = x.BusinessKey,
                WarehouseRef = x.WarehouseRef,
                ParentStructureRef = x.ParentStructureRef,
                Code = x.Code,
                Name = x.Name,
                DisplayOrder = x.DisplayOrder,
                IsActive = x.IsActive
            });

        foreach (var value in values)
        {
            if (items.TryGetValue(value.StructureRef, out var node))
            {
                node.Values.Add(new LocationStructureValueItem
                {
                    LocationStructureValueBusinessKey = value.BusinessKey,
                    StructureRef = value.StructureRef,
                    Code = value.Code,
                    Name = value.Name,
                    DisplayOrder = value.DisplayOrder,
                    IsActive = value.IsActive
                });
            }
        }

        foreach (var item in items.Values)
        {
            if (item.ParentStructureRef.HasValue && items.TryGetValue(item.ParentStructureRef.Value, out var parent))
            {
                parent.Children.Add(item);
            }
        }

        return new GetWarehouseLocationStructureTreeQueryResult
        {
            Items = items.Values
                .Where(x => !x.ParentStructureRef.HasValue || !items.ContainsKey(x.ParentStructureRef.Value))
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.Name)
                .ToList()
        };
    }

    public async Task<GetLocationStructureValuesQueryResult?> GetValuesAsync(Guid structureRef, bool includeInactive = false)
    {
        var items = await _dbContext.Set<LocationStructureValueReadModel>()
            .Where(x => x.StructureRef == structureRef)
            .Where(x => includeInactive || x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .Select(x => new LocationStructureValueItem
            {
                LocationStructureValueBusinessKey = x.BusinessKey,
                StructureRef = x.StructureRef,
                Code = x.Code,
                Name = x.Name,
                DisplayOrder = x.DisplayOrder,
                IsActive = x.IsActive
            })
            .ToListAsync();

        return new GetLocationStructureValuesQueryResult { Items = items };
    }
}
