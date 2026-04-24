namespace Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries.GetByNo;

using OysterFx.AppCore.Shared.Queries;

public class GetInventoryTransactionByNoQuery : IQuery<GetInventoryTransactionByNoQueryResult>
{
    public GetInventoryTransactionByNoQuery(string transactionNo)
    {
        TransactionNo = transactionNo;
    }

    public string TransactionNo { get; }
}
