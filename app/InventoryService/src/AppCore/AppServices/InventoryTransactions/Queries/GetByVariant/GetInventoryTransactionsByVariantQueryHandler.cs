namespace Insurance.InventoryService.AppCore.AppServices.InventoryTransactions.Queries.GetByVariant;

using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries;
using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries.GetByVariant;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetInventoryTransactionsByVariantQueryHandler
    : QueryHandler<GetInventoryTransactionsByVariantQuery, GetInventoryTransactionsByVariantQueryResult>
{
    private readonly IInventoryTransactionQueryRepository _repository;

    public GetInventoryTransactionsByVariantQueryHandler(IInventoryTransactionQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetInventoryTransactionsByVariantQueryResult>> ExecuteAsync(GetInventoryTransactionsByVariantQuery request)
    {
        var items = await _repository.GetByVariantAsync(request.VariantRef);
        return QueryResult<GetInventoryTransactionsByVariantQueryResult>.Success(new GetInventoryTransactionsByVariantQueryResult { Items = items });
    }
}
