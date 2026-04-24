namespace Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries.GetById;

using OysterFx.AppCore.Shared.Queries;

public class GetInventoryTransactionByIdQuery : IQuery<GetInventoryTransactionByIdQueryResult>
{
    public GetInventoryTransactionByIdQuery(Guid transactionId)
    {
        TransactionId = transactionId;
    }

    public Guid TransactionId { get; }
}
