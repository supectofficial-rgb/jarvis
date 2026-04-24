namespace Insurance.InventoryService.AppCore.Shared.Reservations.Queries.GetReservationAllocationsByStockDetailId;

using OysterFx.AppCore.Shared.Queries;

public class GetReservationAllocationsByStockDetailIdQuery : IQuery<GetReservationAllocationsByStockDetailIdQueryResult>
{
    public GetReservationAllocationsByStockDetailIdQuery(Guid stockDetailBusinessKey)
    {
        StockDetailBusinessKey = stockDetailBusinessKey;
    }

    public Guid StockDetailBusinessKey { get; }
}
