namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.GetByCode;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.GetByBusinessKey;
using OysterFx.AppCore.Shared.Queries;

public class GetWarehouseByCodeQuery : IQuery<GetWarehouseByBusinessKeyQueryResult>
{
    public GetWarehouseByCodeQuery(string code)
    {
        Code = code;
    }

    public string Code { get; }
}
