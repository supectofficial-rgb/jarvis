namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetByBusinessKey;

using OysterFx.AppCore.Shared.Queries;

public class GetStockDetailByBusinessKeyQuery : IQuery<GetStockDetailByBusinessKeyQueryResult>
{
    public GetStockDetailByBusinessKeyQuery(Guid stockDetailBusinessKey)
    {
        StockDetailBusinessKey = stockDetailBusinessKey;
    }

    public Guid StockDetailBusinessKey { get; }
}
