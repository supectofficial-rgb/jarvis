namespace Insurance.InventoryService.AppCore.Shared.Returns.Queries.GetByOrder;

using OysterFx.AppCore.Shared.Queries;

public class GetReturnRequestsByOrderQuery : IQuery<GetReturnRequestsByOrderQueryResult>
{
    public GetReturnRequestsByOrderQuery(Guid orderRef)
    {
        OrderRef = orderRef;
    }

    public Guid OrderRef { get; }
}
