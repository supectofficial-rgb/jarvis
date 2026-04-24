namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.GetById;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.GetByBusinessKey;
using OysterFx.AppCore.Shared.Queries;

public class GetLocationByIdQuery : IQuery<GetLocationByBusinessKeyQueryResult>
{
    public GetLocationByIdQuery(Guid locationId)
    {
        LocationId = locationId;
    }

    public Guid LocationId { get; }
}
