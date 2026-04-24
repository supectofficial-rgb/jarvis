namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.GetByCode;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.GetByBusinessKey;
using OysterFx.AppCore.Shared.Queries;

public class GetLocationByCodeQuery : IQuery<GetLocationByBusinessKeyQueryResult>
{
    public GetLocationByCodeQuery(string locationCode)
    {
        LocationCode = locationCode;
    }

    public string LocationCode { get; }
}
