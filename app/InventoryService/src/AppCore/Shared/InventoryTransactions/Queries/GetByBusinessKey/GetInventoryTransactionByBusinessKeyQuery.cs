namespace Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries.GetByBusinessKey;

using OysterFx.AppCore.Shared.Queries;

public class GetInventoryTransactionByBusinessKeyQuery : IQuery<GetInventoryTransactionByBusinessKeyQueryResult>
{
    public GetInventoryTransactionByBusinessKeyQuery(Guid transactionBusinessKey)
    {
        TransactionBusinessKey = transactionBusinessKey;
    }

    public Guid TransactionBusinessKey { get; }
}
