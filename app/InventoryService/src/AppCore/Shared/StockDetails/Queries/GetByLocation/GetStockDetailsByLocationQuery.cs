namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetByLocation;

using OysterFx.AppCore.Shared.Queries;

public class GetStockDetailsByLocationQuery : IQuery<GetStockDetailsByLocationQueryResult>
{
    public GetStockDetailsByLocationQuery(Guid locationRef)
    {
        LocationRef = locationRef;
    }

    public Guid LocationRef { get; }
}
