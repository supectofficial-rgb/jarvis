namespace Insurance.InventoryService.AppCore.AppServices.SourceTracing.Queries.GetByBusinessKey;

using Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.GetByBusinessKey;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetInventorySourceBalanceByBusinessKeyQueryHandler
    : QueryHandler<GetInventorySourceBalanceByBusinessKeyQuery, GetInventorySourceBalanceByBusinessKeyQueryResult>
{
    private readonly IInventorySourceBalanceQueryRepository _repository;

    public GetInventorySourceBalanceByBusinessKeyQueryHandler(IInventorySourceBalanceQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetInventorySourceBalanceByBusinessKeyQueryResult>> ExecuteAsync(GetInventorySourceBalanceByBusinessKeyQuery request)
    {
        var item = await _repository.GetByBusinessKeyAsync(request.SourceBalanceBusinessKey);
        if (item is null)
            return QueryResult<GetInventorySourceBalanceByBusinessKeyQueryResult>.Fail("Source balance was not found.", "NOT_FOUND");

        return QueryResult<GetInventorySourceBalanceByBusinessKeyQueryResult>.Success(item);
    }
}
