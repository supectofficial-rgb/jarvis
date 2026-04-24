namespace Insurance.InventoryService.AppCore.AppServices.InventoryTransactions.Queries.GetById;

using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries;
using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries.GetById;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetInventoryTransactionByIdQueryHandler
    : QueryHandler<GetInventoryTransactionByIdQuery, GetInventoryTransactionByIdQueryResult>
{
    private readonly IInventoryTransactionQueryRepository _repository;

    public GetInventoryTransactionByIdQueryHandler(IInventoryTransactionQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetInventoryTransactionByIdQueryResult>> ExecuteAsync(GetInventoryTransactionByIdQuery request)
    {
        var item = await _repository.GetByIdAsync(request.TransactionId);
        if (item is null)
            return QueryResult<GetInventoryTransactionByIdQueryResult>.Fail("Inventory transaction was not found.", "NOT_FOUND");

        return QueryResult<GetInventoryTransactionByIdQueryResult>.Success(new GetInventoryTransactionByIdQueryResult { Item = item });
    }
}
