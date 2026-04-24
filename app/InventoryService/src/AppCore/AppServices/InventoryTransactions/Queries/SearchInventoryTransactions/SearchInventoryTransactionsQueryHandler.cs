namespace Insurance.InventoryService.AppCore.AppServices.InventoryTransactions.Queries.SearchInventoryTransactions;

using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries;
using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries.SearchInventoryTransactions;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class SearchInventoryTransactionsQueryHandler
    : QueryHandler<SearchInventoryTransactionsQuery, SearchInventoryTransactionsQueryResult>
{
    private readonly IInventoryTransactionQueryRepository _repository;

    public SearchInventoryTransactionsQueryHandler(IInventoryTransactionQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<SearchInventoryTransactionsQueryResult>> ExecuteAsync(SearchInventoryTransactionsQuery request)
    {
        var result = await _repository.SearchAsync(request);
        return QueryResult<SearchInventoryTransactionsQueryResult>.Success(result);
    }
}
