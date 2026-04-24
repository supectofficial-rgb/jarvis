namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.GetByBusinessKey;

using OysterFx.AppCore.Shared.Queries;

public class GetLocationByBusinessKeyQuery : IQuery<GetLocationByBusinessKeyQueryResult>
{
    public GetLocationByBusinessKeyQuery(Guid locationBusinessKey)
    {
        LocationBusinessKey = locationBusinessKey;
    }

    public Guid LocationBusinessKey { get; }
}
