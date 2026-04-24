namespace Insurance.InventoryService.AppCore.AppServices.InventoryTransactions.Queries.GetByBusinessKey;

using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries;
using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries.GetByBusinessKey;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetInventoryTransactionByBusinessKeyQueryHandler
    : QueryHandler<GetInventoryTransactionByBusinessKeyQuery, GetInventoryTransactionByBusinessKeyQueryResult>
{
    private readonly IInventoryTransactionQueryRepository _repository;

    public GetInventoryTransactionByBusinessKeyQueryHandler(IInventoryTransactionQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetInventoryTransactionByBusinessKeyQueryResult>> ExecuteAsync(GetInventoryTransactionByBusinessKeyQuery request)
    {
        var item = await _repository.GetByBusinessKeyAsync(request.TransactionBusinessKey);
        if (item is null)
            return QueryResult<GetInventoryTransactionByBusinessKeyQueryResult>.Fail("Inventory transaction was not found.", "NOT_FOUND");

        return QueryResult<GetInventoryTransactionByBusinessKeyQueryResult>.Success(item);
    }
}
