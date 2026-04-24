namespace Insurance.InventoryService.AppCore.AppServices.InventoryTransactions.Queries.GetByNo;

using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries;
using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries.GetByNo;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetInventoryTransactionByNoQueryHandler
    : QueryHandler<GetInventoryTransactionByNoQuery, GetInventoryTransactionByNoQueryResult>
{
    private readonly IInventoryTransactionQueryRepository _repository;

    public GetInventoryTransactionByNoQueryHandler(IInventoryTransactionQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetInventoryTransactionByNoQueryResult>> ExecuteAsync(GetInventoryTransactionByNoQuery request)
    {
        var item = await _repository.GetByNoAsync(request.TransactionNo);
        if (item is null)
            return QueryResult<GetInventoryTransactionByNoQueryResult>.Fail("Inventory transaction was not found.", "NOT_FOUND");

        return QueryResult<GetInventoryTransactionByNoQueryResult>.Success(new GetInventoryTransactionByNoQueryResult { Item = item });
    }
}
