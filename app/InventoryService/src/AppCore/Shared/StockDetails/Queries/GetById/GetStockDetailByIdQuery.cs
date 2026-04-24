namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetById;

using OysterFx.AppCore.Shared.Queries;

public class GetStockDetailByIdQuery : IQuery<GetStockDetailByIdQueryResult>
{
    public GetStockDetailByIdQuery(Guid stockDetailId)
    {
        StockDetailId = stockDetailId;
    }

    public Guid StockDetailId { get; }
}
