namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.GetWarehouseLookup;

using OysterFx.AppCore.Shared.Queries;

public class GetWarehouseLookupQuery : IQuery<GetWarehouseLookupQueryResult>
{
    public bool IncludeInactive { get; set; }
}
